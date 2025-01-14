using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    [SerializeField] public List<AbilityAnimationController> abilityOverlayAnimator;
    [SerializeField] private ItemPickupAlert itemPickupAlert;
    [SerializeField] private GameObject abilityUIPrefab;
    [SerializeField] private Transform abilityUIParent;
    [SerializeField] private AbilityPickUp abilityPickUpPrefab;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int maxAbilitySlots;

    [HideInInspector] public List<BaseAbility> abilities = new();
    [HideInInspector] public int currAbilitySlots = 0;
    [HideInInspector] public bool swappingAbility = false;

    private List<AbilitySlotUI> abilityUI = new();
    private AbilitySelectUI selectUI;
    private BaseAbility swapAbility;
    private int swapAbilityCharges;
    private PlayerController player;
    private List<Coroutine> abilityCooldownRoutines = new List<Coroutine> { null, null };
    private List<int> charges = new();
    private List<int> maxCharges = new();
    private List<float> abilityCooldowns = new();

    public void InitializeAbilityController()
    {
        player = GetComponent<PlayerController>();
        selectUI = abilityUIParent.GetComponent<AbilitySelectUI>();
        AddAbilitySlot(2);
        SetupAbilities();
    }

    public void AddAbilitySlot(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (currAbilitySlots >= maxAbilitySlots)
                return;

            // add ui
            GameObject obj = Instantiate(abilityUIPrefab, abilityUIParent);
            abilityUI.Add(obj.GetComponent<AbilitySlotUI>());
            if (abilityUI.Count == 10)
                abilityUI[abilityUI.Count - 1].InitAbilityUI("[ 0 ]");
            else
                abilityUI[abilityUI.Count - 1].InitAbilityUI("[ " + abilityUI.Count.ToString() + " ]");
            currAbilitySlots++;
        }
    }

    public void SetupAbilities()
    {
        foreach (BaseAbility ability in GameData.Instance.abilities)
        {
            // add ability
            abilities.Add(ability);
            abilityCooldownRoutines.Add(null);
            charges.Add(ability.abilityCharges);
            maxCharges.Add(ability.abilityMaxCharges);
            abilityCooldowns.Add(0);
            if (abilityUI.Count == 10)
                abilityUI[abilities.Count - 1].InitAbilityUI(ability, "[ 0 ]");
            else
                abilityUI[abilities.Count - 1].InitAbilityUI(ability, "[ " + abilities.Count.ToString() + " ]");
            // init ability
            InitializeAbility(abilities.Count - 1);
        }
    }

    public bool HandleAbilityPickUp(BaseAbility newAbility, int charges)
    {
        itemPickupAlert.DisplayAlert(newAbility);

        if (abilities.Count >= currAbilitySlots)
        {
            swappingAbility = true;
            swapAbility = newAbility;
            swapAbilityCharges = charges;
            selectUI.ShowSelectAbility(true, swapAbility);
            return true;
        }

        // add ability
        AudioManager.Instance.PlayOneShot(Sound.SoundName.PickUp);
        abilities.Add(newAbility);
        abilityCooldownRoutines.Add(null);
        this.charges.Add(charges);
        maxCharges.Add(newAbility.abilityMaxCharges);
        abilityCooldowns.Add(0);
        if (abilityUI.Count == 10)
            abilityUI[abilities.Count - 1].InitAbilityUI(newAbility, "[ 0 ]");
        else
            abilityUI[abilities.Count - 1].InitAbilityUI(newAbility, "[ " + abilities.Count.ToString() + " ]");
        // init ability
        InitializeAbility(abilities.Count - 1);
        return true;
    }

    public void SwapAbility()
    {
        // throw back out ability
        AbilityPickUp ability = Instantiate(abilityPickUpPrefab);
        ability.transform.position = transform.position;
        ability.InitPickup(swapAbility, swapAbility.abilityCharges);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.AbilityClick);

        swappingAbility = false;
        swapAbility = null;
        selectUI.ShowSelectAbility(false, swapAbility);
    }

    public void SwapAbility(int i)
    {
        // throw out old ability
        AbilityPickUp ability = Instantiate(abilityPickUpPrefab);
        ability.transform.position = transform.position;
        ability.InitPickup(abilities[i], charges[i]);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.AbilityClick);
        // add ability
        abilities[i] = swapAbility;
        charges[i] = swapAbilityCharges;
        maxCharges[i] = swapAbility.abilityMaxCharges;
        abilityCooldowns[i] = 0;
        abilityUI[i].InitAbilityUI(swapAbility, "[ " + (i + 1).ToString() + " ]");

        if (abilityCooldownRoutines[i] != null)
        {
            StopCoroutine(abilityCooldownRoutines[i]);
            abilityCooldownRoutines[i] = null;
            abilityUI[i].SetCooldown(0, charges[i], maxCharges[i]);
        }
        // init ability
        InitializeAbility(i);

        swappingAbility = false;
        swapAbility = null;
        selectUI.ShowSelectAbility(false, swapAbility);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.PickUp);
    }

    private void RemoveAbility(int i)
    {
        // remove ability
        abilities.RemoveAt(i);
        charges.RemoveAt(i);
        maxCharges.RemoveAt(i);
        abilityCooldowns.RemoveAt(i);

        if (abilityCooldownRoutines[i] != null)
        {
            StopCoroutine(abilityCooldownRoutines[i]);
            abilityCooldownRoutines[i] = null;
        }

        // re-initialize all abilities
        for (int j = 0; j < abilityUI.Count; j++)
        {
            if (abilityCooldownRoutines[j] != null)
            {
                StopCoroutine(abilityCooldownRoutines[j]);
                abilityCooldownRoutines[j] = null;
            }

            if (j < abilities.Count)
                abilityUI[j].InitAbilityUI(abilities[j], "[ " + (j + 1).ToString() + " ]");
            else
                abilityUI[j].InitAbilityUI("[ " + (j + 1).ToString() + " ]");

            if (abilityCooldowns.Count > j && abilityCooldowns[j] > 0)
                abilityCooldownRoutines[j] = StartCoroutine(AbilityCooldownRoutine(j, abilities[j], abilityCooldowns[j]));
        }
    }

    public void RemoveAllAbilities()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            abilities.RemoveAt(i);
            charges.RemoveAt(i);
            maxCharges.RemoveAt(i);
            abilityCooldowns.RemoveAt(i);

            if (abilityCooldownRoutines[i] != null)
            {
                StopCoroutine(abilityCooldownRoutines[i]);
                abilityCooldownRoutines[i] = null;
            }

            foreach (AbilitySlotUI ui in abilityUI)
                ui.InitAbilityUI(null, "");
        }

        abilities.Clear();
        charges.Clear();
        maxCharges.Clear();
        abilityCooldowns.Clear();
    }

    public void SpawnAbilityPickUp(BaseAbility newAbility, Transform chest)
    {
        AbilityPickUp ability = Instantiate(abilityPickUpPrefab);
        ability.transform.position = chest.position;
        ability.InitPickup(newAbility, newAbility.abilityCharges);
    }

    private void InitializeAbility(int abilityNo)
    {
        if (abilityNo < 0)
        {
            for (int i = 0; i < abilities.Count; i++)
            {
                abilities[i].InitAbility();
                if (charges[i] < maxCharges[i])
                    abilityCooldownRoutines[i] = StartCoroutine(AbilityCooldownRoutine(i, abilities[i]));
            }
        }
        else
        {
            abilities[abilityNo].InitAbility();
            if (charges[abilityNo] < maxCharges[abilityNo])
                abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, abilities[abilityNo]));
        }
    }

    public void HandleAbility(int abilityNo)
    {
        BaseAbility ability = abilities[abilityNo];

        if (ability == null || charges[abilityNo] <= 0)
            return;

        // Emergency Transceiver
        player.ApplyTransceiverBuff();

        // if ability is Area
        if (ability.abilityUseType == BaseAbility.AbilityUseType.Area)
        {
            // get all target objects in area
            Collider2D[] targetColliders = Physics2D.OverlapCircleAll(transform.position, ability.abilityRange, targetLayer);
            List<BaseStats> targetsInArea = new List<BaseStats>();
            foreach (Collider2D col in targetColliders)
            {
                if (col.GetComponent<BaseStats>() != null && col.GetComponent<BaseStats>().health > 0)
                {
                    if (!Physics2D.Raycast(
                        player.transform.position,
                        col.transform.position - player.transform.position,
                        Vector3.Distance(player.transform.position, col.transform.position),
                        groundLayer))
                    {
                        targetsInArea.Add(col.GetComponent<BaseStats>());
                    }
                }
            }
            if (targetsInArea.Count == 0)
                return;

            abilities[abilityNo].OnAbilityUse(null, targetsInArea);
            StartCoroutine(AbilityDurationRoutine(ability, null, targetsInArea));
        }
        // if ability for self or projectile
        else
        {
            abilities[abilityNo].OnAbilityUse(player, null);
            StartCoroutine(AbilityDurationRoutine(ability, player, null));
        }

        charges[abilityNo]--;

        if (ability.isConsumable && charges[abilityNo] == 0)
        {
            RemoveAbility(abilityNo);
            return;
        }

        if (abilityCooldownRoutines[abilityNo] == null)
            abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, ability));
    }

    private IEnumerator AbilityDurationRoutine(BaseAbility ability, BaseStats self, List<BaseStats> targetList)
    {
        yield return new WaitForSeconds(ability.abilityDuration);

        ability.OnAbilityEnd(self, targetList);
    }

    public void HandleAbilityDuration(BaseAbility ability, BaseStats self, List<BaseStats> targetList)
    {
        StartCoroutine(AbilityDurationRoutine(ability, self, targetList));
    }

    private IEnumerator AbilityCooldownRoutine(int abilityNo, BaseAbility ability)
    {
        // track cooldown
        float timer = ability.abilityCooldown;
        while (timer > 0)
        {
            abilityCooldowns[abilityNo] = timer / ability.abilityCooldown;
            abilityUI[abilityNo].SetCooldown(abilityCooldowns[abilityNo], charges[abilityNo], maxCharges[abilityNo]);
            timer -= Time.deltaTime;
            yield return null;
        }

        abilityCooldowns[abilityNo] = 0;

        if (ability.isConsumable)
        {
            abilityUI[abilityNo].SetCooldown(0, charges[abilityNo], maxCharges[abilityNo]);
            abilityCooldownRoutines[abilityNo] = null;
        }
        else
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.AbilityCooldownReset);
            charges[abilityNo]++;
            abilityUI[abilityNo].SetCooldown(0, charges[abilityNo], maxCharges[abilityNo]);
            if (charges[abilityNo] < maxCharges[abilityNo])
                abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, ability));
            else
                abilityCooldownRoutines[abilityNo] = null;
        }
    }

    private IEnumerator AbilityCooldownRoutine(int abilityNo, BaseAbility ability, float cooldown)
    {
        // track cooldown
        float timer = cooldown * ability.abilityCooldown;
        while (timer > 0)
        {
            abilityCooldowns[abilityNo] = timer / ability.abilityCooldown;
            abilityUI[abilityNo].SetCooldown(abilityCooldowns[abilityNo], charges[abilityNo], maxCharges[abilityNo]);
            timer -= Time.deltaTime;
            yield return null;
        }

        abilityCooldowns[abilityNo] = 0;

        if (ability.isConsumable)
        {
            abilityUI[abilityNo].SetCooldown(0, charges[abilityNo], maxCharges[abilityNo]);
            abilityCooldownRoutines[abilityNo] = null;
        }
        else
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.AbilityCooldownReset);
            charges[abilityNo]++;
            abilityUI[abilityNo].SetCooldown(0, charges[abilityNo], maxCharges[abilityNo]);
            if (charges[abilityNo] < maxCharges[abilityNo])
                abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, ability));
            else
                abilityCooldownRoutines[abilityNo] = null;
        }
    }

    public void ResetAbilityCooldowns()
    {
        for (int i = 0; i < abilityCooldownRoutines.Count; i++)
        {
            if (abilityCooldownRoutines[i] == null)
                continue;

            StopCoroutine(abilityCooldownRoutines[i]);

            charges[i]++;
            abilityUI[i].SetCooldown(0, charges[i], maxCharges[i]);
            if (charges[i] < maxCharges[i])
                abilityCooldownRoutines[i] = StartCoroutine(AbilityCooldownRoutine(i, abilities[i]));
            else
                abilityCooldownRoutines[i] = null;
        }
    }

    public void AddAbilityMaxCharges(int amt)
    {
        for(int i = 0; i < maxCharges.Count; i++)
        {
            maxCharges[i] += amt;
            if (charges[i] < maxCharges[i])
                abilityCooldownRoutines[i] = StartCoroutine(AbilityCooldownRoutine(i, abilities[i]));
        }
    }

    public void AddAbilityMaxCharges(int amt, int abilityNo)
    {
        if (abilities[abilityNo] == null)
            return;

        maxCharges[abilityNo] += amt;
        if (charges[abilityNo] < maxCharges[abilityNo])
            abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, abilities[abilityNo]));
    }

    public void AddAbilityMaxCharges(int amt, int numOfAbilities, bool isRandom)
    {
        for (int i = 0; i < numOfAbilities; i++)
        {
            int randomIndex = i;

            if (isRandom)
                randomIndex = Random.Range(0, abilities.Count);

            if (abilities[randomIndex] == null)
                continue;

            maxCharges[randomIndex] += amt;
            if (charges[randomIndex] < maxCharges[randomIndex])
                abilityCooldownRoutines[randomIndex] = StartCoroutine(AbilityCooldownRoutine(randomIndex, abilities[randomIndex]));
        }
    }

    public AbilityAnimationController GetAnimController(string name)
    {
        foreach (AbilityAnimationController controller in abilityOverlayAnimator)
        {
            if (controller.animName.Contains(name))
                return controller;
        }

        return null;
    }
}
