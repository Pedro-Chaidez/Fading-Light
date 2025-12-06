using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyMovementTests {
    [UnityTest]
    public IEnumerator DetectsPlayerInViewRange() {
        var enemyObj = new GameObject("Enemy");
        var enemy = enemyObj.AddComponent<EnemyMovement>();
        enemy.viewDistance = 10f;
        enemy.viewAngle = 90f;

        enemyObj.transform.position = Vector3.zero;
        enemyObj.transform.forward = Vector3.forward;

        var playerObj = new GameObject("Player");
        playerObj.transform.position = new Vector3(0, 0, 5);

        yield return null;

        bool inRange = enemy.IsPlayerInViewRange(playerObj.transform.position, out float distance, out float angle);

        Assert.IsTrue(inRange, "Player should be in view range");
        Assert.AreEqual(5f, distance, 0.1f, "Distance should be 5");
        Assert.AreEqual(0f, angle, 0.1f, "Angle should be 0 (directly in front)");

        Object.Destroy(enemyObj);
        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator DoesNotDetectPlayerOutOfRange() {
        // Arrange
        var enemyObj = new GameObject("Enemy");
        var enemy = enemyObj.AddComponent<EnemyMovement>();
        enemy.viewDistance = 10f;
        enemy.viewAngle = 90f;

        enemyObj.transform.position = Vector3.zero;
        enemyObj.transform.forward = Vector3.forward;

        var playerObj = new GameObject("Player");
        playerObj.transform.position = new Vector3(0, 0, 15);

        yield return null;

        bool inRange = enemy.IsPlayerInViewRange(playerObj.transform.position, out float distance, out float angle);

        Assert.IsFalse(inRange, "Player should be out of view range");

        Object.Destroy(enemyObj);
        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator DoesNotDetectPlayerBehind() {

        var enemyObj = new GameObject("Enemy");
        var enemy = enemyObj.AddComponent<EnemyMovement>();
        enemy.viewDistance = 10f;
        enemy.viewAngle = 90f;

        enemyObj.transform.position = Vector3.zero;
        enemyObj.transform.forward = Vector3.forward;

        var playerObj = new GameObject("Player");
        playerObj.transform.position = new Vector3(0, 0, -5);

        yield return null;

        bool inRange = enemy.IsPlayerInViewRange(playerObj.transform.position, out float distance, out float angle);

        Assert.IsFalse(inRange, "Player behind should not be in view range");

        Object.Destroy(enemyObj);
        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator DetectsFacingPlayer() {
        var enemyObj = new GameObject("Enemy");
        var enemy = enemyObj.AddComponent<EnemyMovement>();
        enemy.facingThreshold = 0.9f;

        enemyObj.transform.position = Vector3.zero;
        enemyObj.transform.forward = Vector3.forward;

        var playerObj = new GameObject("Player");
        playerObj.transform.position = new Vector3(0, 0, 5);
        playerObj.transform.forward = Vector3.back;

        yield return null;

        bool facing = enemy.AreFacingEachOther(playerObj.transform, out float enemyDot, out float playerDot);

        Assert.IsTrue(facing, "Enemy and player should be facing each other");
        Assert.Greater(enemyDot, 0.9f, "Enemy should be facing player");
        Assert.Greater(playerDot, 0.9f, "Player should be facing enemy");

        Object.Destroy(enemyObj);
        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator StopChasing() {
        var enemyObj = new GameObject("Enemy");
        var enemy = enemyObj.AddComponent<EnemyMovement>();
        enemy.aggroLost = 3f;

        yield return null;

        enemy.TestSetChasing(true);
        enemy.TestSetOutOfSight(0f);

        enemy.TestSetOutOfSight(3.5f);
        enemy.UpdateChaseState();

        Assert.IsFalse(enemy.chasing, "Should stop chasing after aggro lost time");
        Assert.AreEqual(0f, enemy.outOfSight, "Out of sight timer should reset");

        Object.Destroy(enemyObj);
    }

    [Test]
    public void Enemy_CalculatesRandomWalkPoint() {
        var enemyObj = new GameObject("Enemy");
        var enemy = enemyObj.AddComponent<EnemyMovement>();
        enemy.walkPointRange = 10f;
        enemyObj.transform.position = new Vector3(5, 0, 5);

        enemy.SetWalkPoint();
        Vector3 walkPoint = enemy.walkPoint;

        float distance = Vector3.Distance(enemyObj.transform.position, walkPoint);
        Assert.LessOrEqual(distance, 10f * 1.42f, "Walk point should be within range"); // sqrt(2) for diagonal

        Object.Destroy(enemyObj);
    }
}