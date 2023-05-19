using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
// 追記
using System.Collections;
using System;
using System.Drawing;
using System.Collections.Generic;

public class MoveSpeedChanger : MonoBehaviour
{
    [SerializeField] ContinuousMoveProviderBase continuousMoveProvider;
    [SerializeField] float playerBaseSpeed = 4;
	// 追加
	public float SpeedTime = 10.0f;
	public float SpeedUp = 1.2f;
	private static int cnt = 0;

    public void MultiplyPlayerSpeed(float multiplier)
    {
        if(continuousMoveProvider != null)
        {
            continuousMoveProvider.moveSpeed *= multiplier;
        }
    }

    public void DividePlayerSpeed(float multiplier)
    {
        if(continuousMoveProvider != null)
        {
            continuousMoveProvider.moveSpeed /= multiplier;
        }
    }

    public void EnableMovement()
    {
        continuousMoveProvider.enabled = true;
    }

    public void DisableMovement()
    {
        continuousMoveProvider.enabled = false;
    }

    void Start()
    {
        if(continuousMoveProvider != null)
        {
            continuousMoveProvider.moveSpeed = playerBaseSpeed;
        }
		// 追記
		cnt = 0;
    }

	// 追加
	public void accel(){
		if(cnt++ == 0) StartCoroutine(startAccel());
	}

	public IEnumerator startAccel(){
		MultiplyPlayerSpeed(SpeedUp);
		while(cnt > 0){
			yield return new WaitForSeconds(SpeedTime);
			cnt--;
		}
		DividePlayerSpeed(SpeedUp);
		yield return null;
	}

}
