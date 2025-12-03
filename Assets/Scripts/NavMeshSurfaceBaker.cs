using Unity.AI.Navigation;
using UnityEngine;
   using UnityEngine.AI; // Required for NavMeshSurface

   public class NavMeshSurfaceBaker : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;

    void Start()
    {
        // You can also get the component dynamically if it's on the same GameObject
        // navMeshSurface = GetComponent<NavMeshSurface>(); 
        navMeshSurface = GetComponent<NavMeshSurface>();
        if (navMeshSurface != null)
        {
            BakeNavMesh();
        }
        else
        {
            Debug.LogError("NavMeshSurface not assigned or found!");
        }
    }

    public void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // This bakes the NavMesh
        }
    }

    // You might want to rebake when certain objects change or are added/removed
    // public void UpdateNavMesh()
    // {
    //     if (navMeshSurface != null)
    //     // Consider using navMeshSurface.UpdateNavMesh(navMeshData) for partial updates
    //     // if you are only modifying a specific region.
    //     {
    //         navMeshSurface.RemoveData(); // Clear old data if completely rebuilding
    //         navMeshSurface.BuildNavMesh(); 
    //         Debug.Log("NavMesh updated successfully!");
    //     }
    // }
}
