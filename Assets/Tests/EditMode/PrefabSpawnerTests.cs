using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PrefabSpawnerTests {
    [Test]
    public void SpawnsPrefabAtCorrectPosition() {
        var spawnerObj = new GameObject("Spawner");
        spawnerObj.transform.position = new Vector3(5f, 10f, 15f);
        var spawner = spawnerObj.AddComponent<PrefabSpawner>();

        var respawnPointField = typeof(PrefabSpawner).GetField("respawnPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        respawnPointField.SetValue(spawner, spawnerObj.transform);

        var prefab = new GameObject("TestPrefab");

        var field = typeof(PrefabSpawner).GetField("prefabToSpawn",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(spawner, prefab);

        spawner.RespawnPrefab();

        var spawnedObject = GameObject.Find("TestPrefab(Clone)");
        Assert.IsNotNull(spawnedObject, "Prefab should be spawned");
        Assert.AreEqual(new Vector3(5f, 10f, 15f), spawnedObject.transform.position,
            "Spawned prefab should be at spawner position");

        Object.DestroyImmediate(spawnerObj);
        Object.DestroyImmediate(prefab);
        Object.DestroyImmediate(spawnedObject);
    }

    [Test]
    public void SpawnsPrefabWithCorrectRotation() {
        var spawnerObj = new GameObject("Spawner");
        spawnerObj.transform.rotation = Quaternion.Euler(45f, 90f, 180f);
        var spawner = spawnerObj.AddComponent<PrefabSpawner>();

        var respawnPointField = typeof(PrefabSpawner).GetField("respawnPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        respawnPointField.SetValue(spawner, spawnerObj.transform);

        var prefab = new GameObject("TestPrefab");

        var field = typeof(PrefabSpawner).GetField("prefabToSpawn",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(spawner, prefab);

        spawner.RespawnPrefab();

        var spawnedObject = GameObject.Find("TestPrefab(Clone)");
        Assert.IsNotNull(spawnedObject, "Prefab should be spawned");

        Assert.That(
            Quaternion.Angle(spawnerObj.transform.rotation, spawnedObject.transform.rotation),
            Is.LessThan(0.01f),
            "Spawned prefab should have spawner rotation"
        );
        Object.DestroyImmediate(spawnerObj);
        Object.DestroyImmediate(prefab);
        Object.DestroyImmediate(spawnedObject);
    }

    [Test]
    public void SpawnedPrefabHasCorrectParent() {
        var spawnerObj = new GameObject("Spawner");
        var spawner = spawnerObj.AddComponent<PrefabSpawner>();

        var respawnPointField = typeof(PrefabSpawner).GetField("respawnPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        respawnPointField.SetValue(spawner, spawnerObj.transform);

        var prefab = new GameObject("TestPrefab");

        var field = typeof(PrefabSpawner).GetField("prefabToSpawn",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(spawner, prefab);

        spawner.RespawnPrefab();

        var spawnedObject = GameObject.Find("TestPrefab(Clone)");
        Assert.IsNotNull(spawnedObject, "Prefab should be spawned");
        Assert.AreEqual(spawnerObj.transform, spawnedObject.transform.parent,
            "Spawned prefab should be child of spawner");

        Object.DestroyImmediate(spawnerObj);
        Object.DestroyImmediate(prefab);
    }

    [Test]
    public void SpawnsMultiplePrefabsOnMultipleCalls() {
        var spawnerObj = new GameObject("Spawner");
        var spawner = spawnerObj.AddComponent<PrefabSpawner>();

        var respawnPointField = typeof(PrefabSpawner).GetField("respawnPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        respawnPointField.SetValue(spawner, spawnerObj.transform);

        var prefab = new GameObject("TestPrefab");

        var field = typeof(PrefabSpawner).GetField("prefabToSpawn",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(spawner, prefab);

        spawner.RespawnPrefab();
        spawner.RespawnPrefab();
        spawner.RespawnPrefab();

        var spawnedObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;
        foreach (var obj in spawnedObjects) {
            if (obj != null && obj.name == "TestPrefab(Clone)")
                count++;
        }

        Assert.AreEqual(3, count, "Should spawn 3 prefabs");

        Object.DestroyImmediate(spawnerObj);
        Object.DestroyImmediate(prefab);
    }

    [Test]
    public void DoesNotCrashWhenPrefabIsNull() {
        var spawnerObj = new GameObject("Spawner");
        var spawner = spawnerObj.AddComponent<PrefabSpawner>();

        var respawnPointField = typeof(PrefabSpawner).GetField("respawnPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        respawnPointField.SetValue(spawner, spawnerObj.transform);

        LogAssert.Expect(LogType.Error, "PrefabToSpawn or RespawnPoint is not assigned in the Inspector!");
        spawner.RespawnPrefab();

        Assert.IsTrue(true, "Should not crash when prefab is null");

        Object.DestroyImmediate(spawnerObj);
    }

    [Test]
    public void SpawnsPrefabWithComponents() {
        var spawnerObj = new GameObject("Spawner");
        var spawner = spawnerObj.AddComponent<PrefabSpawner>();

        var respawnPointField = typeof(PrefabSpawner).GetField("respawnPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        respawnPointField.SetValue(spawner, spawnerObj.transform);

        var prefab = new GameObject("TestPrefab");
        prefab.AddComponent<BoxCollider>();
        prefab.AddComponent<Rigidbody>();

        var field = typeof(PrefabSpawner).GetField("prefabToSpawn",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(spawner, prefab);

        spawner.RespawnPrefab();

        var spawnedObject = GameObject.Find("TestPrefab(Clone)");
        Assert.IsNotNull(spawnedObject, "Prefab should be spawned");
        Assert.IsNotNull(spawnedObject.GetComponent<BoxCollider>(),
            "Spawned prefab should have BoxCollider");
        Assert.IsNotNull(spawnedObject.GetComponent<Rigidbody>(),
            "Spawned prefab should have Rigidbody");

        Object.DestroyImmediate(spawnerObj);
        Object.DestroyImmediate(prefab);
    }
}