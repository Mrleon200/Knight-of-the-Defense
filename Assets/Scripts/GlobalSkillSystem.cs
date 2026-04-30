using UnityEngine;

// SETUP: Tao GameObject "SkillManager" → gan script nay
// UI Button Meteor        → OnClick → GlobalSkillSystem.Instance.ActivateMeteor()
// UI Button Reinforcement → OnClick → GlobalSkillSystem.Instance.ActivateReinforcement()
public class GlobalSkillSystem : MonoBehaviour
{
    public static GlobalSkillSystem Instance { get; private set; }

    [Header("Meteor")]
    public GameObject meteorEffectPrefab;
    public float      meteorDamage   = 150f;
    public float      meteorRadius   = 2.5f;
    public float      meteorCooldown = 20f;

    [Header("Reinforcement")]
    public GameObject tempSoldierPrefab;
    public int        reinforcementCount    = 3;
    public float      reinforcementDuration = 15f;
    public float      reinforcementCooldown = 25f;

    private float  _nextMeteorTime        = 0f;
    private float  _nextReinforcementTime = 0f;
    private string _pendingSkill          = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (_pendingSkill == "") return;
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        { _pendingSkill = ""; return; }
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;

        if (_pendingSkill == "meteor")    CastMeteor(pos);
        if (_pendingSkill == "reinforce") CastReinforcement(pos);
        _pendingSkill = "";
    }

    public void ActivateMeteor()
    {
        if (Time.time < _nextMeteorTime) { Debug.Log($"[Meteor] Hoi chieu: {_nextMeteorTime - Time.time:F1}s"); return; }
        _pendingSkill = "meteor";
        Debug.Log("[Meteor] Click vi tri muon tha.");
    }

    private void CastMeteor(Vector3 pos)
    {
        if (meteorEffectPrefab != null) Instantiate(meteorEffectPrefab, pos, Quaternion.identity);
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, meteorRadius);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            EnemyStats s = hit.GetComponent<EnemyStats>();
            if (s != null) s.TakeDamage(meteorDamage, DamageType.Magic);
        }
        _nextMeteorTime = Time.time + meteorCooldown;
    }

    public void ActivateReinforcement()
    {
        if (Time.time < _nextReinforcementTime) { Debug.Log($"[Reinforce] Hoi chieu: {_nextReinforcementTime - Time.time:F1}s"); return; }
        _pendingSkill = "reinforce";
        Debug.Log("[Reinforce] Click vi tri muon spawn linh.");
    }

    private void CastReinforcement(Vector3 pos)
    {
        if (tempSoldierPrefab == null) return;
        for (int i = 0; i < reinforcementCount; i++)
        {
            float   angle  = (360f / reinforcementCount) * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 0.8f;
            GameObject s   = Instantiate(tempSoldierPrefab, pos + offset, Quaternion.identity);
            Destroy(s, reinforcementDuration);
        }
        _nextReinforcementTime = Time.time + reinforcementCooldown;
    }

    public float MeteorCooldownPercent()
    {
    return Mathf.Clamp01(1f - (_nextMeteorTime - Time.time) / meteorCooldown);
    }

    public float ReinforceCooldownPercent()
    {
    return Mathf.Clamp01(1f - (_nextReinforcementTime - Time.time) / reinforcementCooldown);
    }
}
