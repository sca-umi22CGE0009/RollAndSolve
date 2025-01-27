using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SasakiShu
/// カメラ操作
/// </summary>
public class CameraController : MonoBehaviour
{
    private float startSize = 5.0f;
    [SerializeField] private float repositionSpeed = 10.0f;
    private float beforeSpeed;
    private float zoomInSpeed;
    private bool isSuddenDeceleration;
    private GameObject Player;
    [SerializeField] private GameObject LimitLeft;
    [SerializeField] private GameObject LimitRight;
    private float centerPosition;
    private float repositionRight;//右側のターンポジション
    private float repositionLeft;//左側のターンポジション
    private float setPositionRight;
    private float setPositionLeft;
    private Camera mainCam;
    private PlayerController playercontroller;

    public enum CameraMoveState { Move, Stop, Turn, LimitRight, LimitLeft}
    public CameraMoveState cameraMoveState;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        mainCam = Camera.main;
        mainCam.orthographicSize = startSize;
        playercontroller = FindObjectOfType<PlayerController>();
        //mainCam.transform.position = new Vector3(transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
        //CameraReposition();
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
        if (playercontroller.playerstate == PlayerController.PlayerState.Capsule)
            Zoom();
        if (cameraMoveState == CameraMoveState.Turn)
            CameraReposition();

        //人型に戻った時にカメラのorthographicSizeを初期値まで戻す
        else if (mainCam.orthographicSize >= 5.0f)
        {
            mainCam.orthographicSize -= Time.deltaTime;
            mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
            if (mainCam.orthographicSize < 5.0f)
            {
                mainCam.orthographicSize = 5.0f;
            }
        }
    }

    private void CameraMove()
    {
        centerPosition = mainCam.transform.position.x;
        repositionRight = mainCam.transform.position.x + 3.0f;
        repositionLeft = mainCam.transform.position.x - 3.0f;
        setPositionRight = mainCam.transform.position.x + 2.0f;
        setPositionLeft = mainCam.transform.position.x - 2.0f;

        //Playerが右に進んでいるとき
        if (playercontroller.directionstate == PlayerController.DirectionState.Right)
        {
            //右限界
            if (mainCam.ViewportToWorldPoint(Vector2.one).x > LimitRight.transform.position.x)
            {
                //mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
                cameraMoveState = CameraMoveState.LimitRight;
                return;
            }

            //�@
            if (Player.transform.position.x <= setPositionLeft)
            {
                cameraMoveState = CameraMoveState.Stop;
            }

            if (cameraMoveState == CameraMoveState.LimitRight)
            {
                return;
            }

            //�B
            else if (Player.transform.position.x > setPositionLeft && Player.transform.position.x < centerPosition)
            {
                if (cameraMoveState != CameraMoveState.Turn)
                {
                    transform.position += new Vector3(playercontroller.speed, 0, 0) * Time.deltaTime;
                    //mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
                }
            }

            //�C
            else if (Player.transform.position.x > centerPosition && Player.transform.position.x < repositionRight)
            {
                return;
            }

            //�D
            else if (Player.transform.position.x > repositionRight)
            {
                cameraMoveState = CameraMoveState.Turn;
            }
        }

        //Playerが左に進んでいるとき
        else if(playercontroller.directionstate == PlayerController.DirectionState.Left)
        {
            //左限界
            if (mainCam.ViewportToWorldPoint(Vector2.zero).x < LimitLeft.transform.position.x)
            {
                //mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
                cameraMoveState = CameraMoveState.LimitLeft;

                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    Debug.Log("�D");
                }
                return;
            }

            //�D
            if (Player.transform.position.x >= setPositionRight)
            {
                cameraMoveState = CameraMoveState.Stop;
            }

            if (cameraMoveState == CameraMoveState.LimitLeft)
            {
                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    Debug.Log("�D");
                }
                return;
            }

            //�B
            else if (Player.transform.position.x > centerPosition && Player.transform.position.x < setPositionRight)
            {
                if (cameraMoveState != CameraMoveState.Turn)
                {
                    transform.position += new Vector3(playercontroller.speed, 0, 0) * Time.deltaTime;
                    //mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
                }
            }

            //�A
            else if (Player.transform.position.x < centerPosition && Player.transform.position.x > setPositionLeft)
            {
                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    Debug.Log("�A");
                }
                return;
            }

            //�@
            else if (Player.transform.position.x < repositionLeft)
            {
                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    Debug.Log("�@");
                }
                cameraMoveState = CameraMoveState.Turn;
            }
        }
        mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
    }

    //カメラのずれを直す
    private void CameraReposition()
    {
        if (playercontroller.directionstate == PlayerController.DirectionState.Right)
        {
            transform.position += new Vector3(playercontroller.speed + repositionSpeed, 0, 0) * Time.deltaTime;
            mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
        }
        else if (playercontroller.directionstate == PlayerController.DirectionState.Left)
        {
            transform.position += new Vector3(playercontroller.speed - repositionSpeed, 0, 0) * Time.deltaTime;
            mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
        }
    }

    private void Zoom()
    {
        //右限界
        if (cameraMoveState == CameraMoveState.LimitRight)
        {
            if (!isSuddenDeceleration)
            {
                zoomInSpeed = (mainCam.orthographicSize - startSize) * 2;
                isSuddenDeceleration = true;
            }
            mainCam.orthographicSize -= zoomInSpeed * Time.deltaTime;
            mainCam.transform.position = new Vector3(
                mainCam.ViewportToWorldPoint(Vector2.zero).x + ((LimitRight.transform.position.x - mainCam.ViewportToWorldPoint(Vector2.zero).x) / 2),
                mainCam.orthographicSize - startSize,
                -10.0f);
            if (mainCam.orthographicSize <= startSize)
            {
                mainCam.orthographicSize = startSize;
                isSuddenDeceleration = false;
            }
            return;
        }

        //左限界
        if (cameraMoveState == CameraMoveState.LimitLeft)
        {
            if (!isSuddenDeceleration)
            {
                zoomInSpeed = (mainCam.orthographicSize - startSize) * 2;
                isSuddenDeceleration = true;
            }
            mainCam.orthographicSize -= zoomInSpeed * Time.deltaTime;
            mainCam.transform.position = new Vector3(
                LimitLeft.transform.position.x + ((mainCam.ViewportToWorldPoint(Vector2.one).x - LimitLeft.transform.position.x) / 2),
                mainCam.orthographicSize - startSize,
                -10.0f
                );
            if (mainCam.orthographicSize <= startSize)
            {
                mainCam.orthographicSize = startSize;
                isSuddenDeceleration = false;
            }
            return;
        }

        //急に止まった時に滑らかにズームイン
        //if (Mathf.Abs(beforeSpeed) - Mathf.Abs(playercontroller.speed) >= 0.5f || isSuddenDeceleration)
        //{
        //    if (!isSuddenDeceleration)
        //    {
        //        zoomInSpeed = (mainCam.orthographicSize - startSize) * 2;
        //        isSuddenDeceleration = true;
        //    }

        //    mainCam.orthographicSize -= zoomInSpeed * Time.deltaTime;
        //    mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
        //    if (mainCam.orthographicSize <= startSize)
        //    {
        //        mainCam.orthographicSize = startSize;
        //        isSuddenDeceleration = false;
        //        beforeSpeed = 0.0f;
        //        zoomInSpeed = 0.0f;
        //    }
        //    return;
        //}

        //一定速度を超えるとズームアウト
        if (Mathf.Abs(playercontroller.speed) > Mathf.Abs(playercontroller.CapsuleLimitSpeed) * 0.8f)
        {
            if (mainCam.orthographicSize <= 6.0f)
            {
                mainCam.orthographicSize = mainCam.orthographicSize + (3.0f * Time.deltaTime);
            }
            //mainCam.orthographicSize = startSize + ((Mathf.Abs(playercontroller.speed) - (Mathf.Abs(playercontroller.CapsuleLimitSpeed) * 0.8f)) / 1.5f);
        }
        if (Mathf.Abs(playercontroller.speed) < Mathf.Abs(playercontroller.CapsuleLimitSpeed) * 0.4f)
        {
            if (mainCam.orthographicSize > 5.0f)
            {
                mainCam.orthographicSize = mainCam.orthographicSize - (1.0f * Time.deltaTime);
            }
        }
        mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.orthographicSize - startSize, -10.0f);
        beforeSpeed = playercontroller.speed;
    }

    public void HitShake(float duration, float magnitude)
    {
        StartCoroutine(CameraShake( duration, magnitude));
    }

    private IEnumerator CameraShake( float duration, float magnitude)
    {
        var pos = transform.localPosition;
        var elapsed = 0.0f;
        Debug.Log(mainCam.orthographicSize);

        while (elapsed < duration)
        {
            //var x = pos.x + Random.Range(-1.0f, 1.0f) * magnitude;
            var y = pos.y + Random.Range(-1.0f, 1.0f) * magnitude;

            transform.localPosition = new Vector3(mainCam.transform.position.x, y, pos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
