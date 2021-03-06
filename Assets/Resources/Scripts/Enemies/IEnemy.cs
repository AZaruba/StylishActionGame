﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy {

    Vector3 CalculateMovement();
    void Attack();

    void GetHit(int damage);
    void ResetHit();
    void OnDefeat();
    void OnPlayerDefeated();

    Vector3 SendPosition();
}
