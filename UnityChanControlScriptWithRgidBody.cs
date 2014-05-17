//
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

// 必要なコンポーネントの列記
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]

public class UnityChanControlScriptWithRgidBody : MonoBehaviour
{
	public enum UnityChanState
	{
		Glide,
		Hover,
		Glide2Hover,
		Hover2Gride
	}

	public UnityChanState uState = UnityChanState.Hover;

	public float animSpeed = 1.5f;				// アニメーション再生速度設定
	public float lookSmoother = 3.0f;			// a smoothing setting for camera motion
	public bool useCurves = true;				// Mecanimでカーブ調整を使うか設定する
												// このスイッチが入っていないとカーブは使われない
	public float useCurvesHeight = 0.5f;		// カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）

	// 以下キャラクターコントローラ用パラメタ
	// 前進速度
	public float forwardSpeed = 1.0f;
	public float maxForwardSpeed = 5.0f;
	public float forwardAccel = 0.2f;
	public float forwardHover = 0.1f;
	// 後退速度
	public float backwardSpeed = 2.0f;
	// 旋回速度
	public float rotateSpeed = 2.0f;
	// ジャンプ威力
	public float jumpPower = 3.0f; 
	// キャラクターコントローラ（カプセルコライダ）の参照
	private CapsuleCollider col;
	private Rigidbody rb;
	// キャラクターコントローラ（カプセルコライダ）の移動量
	private Vector3 velocity;
	// CapsuleColliderで設定されているコライダのHeiht、Centerの初期値を収める変数
	private float orgColHight;
	private Vector3 orgVectColCenter;
	
	private Animator anim;							// キャラにアタッチされるアニメーターへの参照
	private AnimatorStateInfo currentBaseState;			// base layerで使われる、アニメーターの現在の状態の参照

	private GameObject cameraObject;	// メインカメラへの参照

// 初期化
	void Start ()
	{
		// Animatorコンポーネントを取得する
		anim = GetComponent<Animator>();
		// CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
		col = GetComponent<CapsuleCollider>();
		rb = GetComponent<Rigidbody>();
		//メインカメラを取得する
		cameraObject = GameObject.FindWithTag("MainCamera");
		// CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
		orgColHight = col.height;
		orgVectColCenter = col.center;
}
	
	
// 以下、メイン処理.リジッドボディと絡めるので、FixedUpdate内で処理を行う.
	void FixedUpdate ()
	{
		float h = Input.GetAxis("Horizontal");				// 入力デバイスの水平軸をhで定義
		float v = Input.GetAxis("Vertical");				// 入力デバイスの垂直軸をvで定義
		// 以下、キャラクターの移動処理
		velocity = new Vector3(0, 0, v);		// 上下のキー入力からZ軸方向の移動量を取得
		// キャラクターのローカル空間での方向に変換
		velocity = transform.TransformDirection(velocity);
		// 上下のキー入力でキャラクターを移動させる
		transform.localPosition += velocity * Time.fixedDeltaTime;

		// 左右のキー入力でキャラクタをY軸で旋回させる
		transform.Rotate(0, h * rotateSpeed, 0);	

		if (uState == UnityChanState.Glide)
		{
			rb.useGravity = false;
			//滑空状態
			//徐々に加速
			if (forwardSpeed < maxForwardSpeed)
			{
				forwardSpeed = forwardSpeed + forwardAccel;
				anim.SetFloat("Speed",forwardSpeed);
			}
			else if (forwardSpeed != maxForwardSpeed)
			{
				forwardSpeed = maxForwardSpeed;
				anim.SetFloat("Speed",forwardSpeed);
			}
			velocity = new Vector3(0, 0, forwardSpeed);
			velocity = transform.TransformDirection(velocity);
			transform.localPosition += velocity * Time.fixedDeltaTime;

			//クリックしたらホバリング状態へ
			if (Input.GetButton("Fire1"))
			{
				anim.SetBool("glide2hover",true);
				uState = UnityChanState.Glide2Hover;
			}
		}
		else if (uState == UnityChanState.Hover)
		{
			anim.SetBool("hover",true);
			rb.useGravity = false;
			//ホバリング状態
			if (forwardSpeed > forwardHover)
			{
				forwardSpeed = forwardSpeed * 0.8f;
				anim.SetFloat("Speed",forwardSpeed);
			}
			else if (forwardSpeed != forwardHover)
			{
				forwardSpeed = forwardHover;
				anim.SetFloat("Speed",forwardSpeed);
			}
			velocity = new Vector3(0, 0, forwardSpeed);
			velocity = transform.TransformDirection(velocity);
			transform.localPosition += velocity * Time.fixedDeltaTime;
			//クリックしたら滑空状態へ
			if (Input.GetButton("Fire1"))
			{
				anim.SetBool("hover2glide",true);
				uState = UnityChanState.Hover2Gride;
			}
		}
		//首を無理やり回す
		//Transform neckTransform = GameObject.Find ("Character1_Neck").GetComponent< Transform> ();
		//neckTransform.Rotate(0,1,0);
		//Transform neckTransform = GameObject.Find ("Character1_LeftShoulder").GetComponent< Transform> ();
		
		//neckTransform.LookAt(Camera.main.transform.position);
		//neckTransform.LookAt(Camera.main.transform.localPosition);
		//Quaternion quaternionNeck = Quaternion.AngleAxis (90.0f, Vector3.up);
		//neckTransform.localRotation = quaternionNeck;
	}
	void Update()
	{

	}
	void OnGUI()
	{
	}


	// キャラクターのコライダーサイズのリセット関数
	void resetCollider()
	{
	// コンポーネントのHeight、Centerの初期値を戻す
		col.height = orgColHight;
		col.center = orgVectColCenter;
	}
}
