using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private InitializeMonoBeh[] initializes;

    private void Awake()
    {
        foreach (var item in initializes)
        {
            if (item.gameObject.activeSelf)
            {
                item.Init();
            }
        }
    }
}