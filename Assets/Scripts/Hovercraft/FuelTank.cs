using UnityEngine;

public class FuelTank : MonoBehaviour
{
    [field: SerializeField] public float FuelAmount { get; private set; }
    [SerializeField] private float fuelBurnSpeed = 1;

    public void BurnFuel()
    {
        FuelAmount -= Time.deltaTime * fuelBurnSpeed;
        FuelAmount = Mathf.Max(0, FuelAmount);
    }

    private void OnTriggerEnter(Collider other)
    {
        var booster = other.GetComponent<FuelBooster>();

        if (booster)
        {
            FuelAmount += booster.amount;

            Destroy(other.gameObject);
        }
    }
}