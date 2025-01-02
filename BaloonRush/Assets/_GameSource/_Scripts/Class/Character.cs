using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    public GameObject balonModel;
    public GameObject crown;
    public int id;

    [HideInInspector] public float distance;
    [HideInInspector] public bool canFly;
    [HideInInspector] public bool canFall;

    [SerializeField] float runningspeed;
    [SerializeField] ParticleSystem fuelFx;
    [SerializeField] ParticleSystem groundFx;
    [SerializeField] ParticleSystem runFx;

    Jetpack myJetpack;

    TextMesh nameText;
    Animator playerAnimator;
    private Rigidbody _rb;
    private CapsuleCollider _myCollider;

    Vector3 runningVector;
    LayerMask mask;
    private Vector3 _swimPos;
    public bool _onGround;
    public bool _isAlive;
    public bool _isWin;

    // ============================ *** START

    private void Awake()
    {
        _onGround = true;
        playerAnimator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
        _myCollider = GetComponent<CapsuleCollider>();
        myJetpack = GetComponentInChildren<Jetpack>();
        canFly = true;
        mask = LayerMask.GetMask("Platform");
        nameText = GetComponentInChildren<TextMesh>();
    }

    private void FixedUpdate()
    {
        if (_isAlive && !_isWin)
            Run();
    }

    private void OnDisable()
    {
        runFx.Stop();
    }

    private void Update()
    {
        if (_isAlive && !_isWin)
        {
            distance = Vector3.Distance(transform.position, GameManager.Instance.finish.position);
            if (!myJetpack.enabled && !Raycast() && myJetpack.GetFuel() >= 0 && !_onGround)
                ControllJetpack(true);
            else if (myJetpack.enabled && Raycast())
                ControllJetpack(false);
        }          
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && _isAlive && !_isWin && !_onGround)
        {
            Vector3 _normal = collision.contacts[collision.contactCount - 1].normal;
            _onGround = true;
            groundFx.Play();
            ControllWallCollide(_normal);
            StartRunning();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && _isAlive)
            _onGround = false;
    }

    public void StartRunning()
    {
        _isAlive = true;
        runFx.Play();
        playerAnimator.Play("Run");
        playerAnimator.SetBool("isFlying", false);
    }

    public void BonusTime()
    {
        ControllJetpack(true);
        myJetpack.BonusTime();
        Destroy(this);
    }
   
    // =====================================

    public void Win()
    {
        _isWin = true;
        myJetpack.enabled = false;
        playerAnimator.SetBool("isFlying", false);
        playerAnimator.Play("Win");
        runFx.Stop();
    }

    public void Lost()
    {
        _isAlive = false;
        playerAnimator.SetTrigger("Fail");
        runFx.Stop();
    }

    public void ControllJetpack(bool isOpen)
    {
        myJetpack.enabled = isOpen;
        if (isOpen)
        {
            runFx.Stop();
            playerAnimator.SetBool("isFlying", true);

            balonModel.transform.DOScaleX(balonModel.transform.localScale.x - 0.005f, 0.2f);
            balonModel.transform.DOScaleY(balonModel.transform.localScale.y - 0.002f, 0.2f);
            balonModel.transform.DOScaleZ(balonModel.transform.localScale.z - 0.005f, 0.2f);
        }
    }

    public void Turning(float rotateValue)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotateValue, transform.localEulerAngles.z);
    }

    public void GetFuel()
    {
        fuelFx.Play();
        myJetpack.AddFuel(.5f);

        balonModel.transform.DOScaleX(balonModel.transform.localScale.x + 0.05f, 0.2f);
        balonModel.transform.DOScaleY(balonModel.transform.localScale.y + 0.02f, 0.2f);
        balonModel.transform.DOScaleZ(balonModel.transform.localScale.z + 0.05f, 0.2f);

        if(gameObject.CompareTag("Player"))
            GameManager.Instance.FillFuelBar(myJetpack.GetFuelValue());
    }

    private void Run()
    {
        runningVector = transform.forward * runningspeed * Time.deltaTime;
        transform.position += runningVector;
    }
    
    // =====================================

    bool Raycast()
    {
        RaycastHit _hit;
        if (Physics.Raycast(transform.position + transform.up * .5f, -Vector3.up, 100f, mask))
        {
            Debug.DrawRay(transform.position + transform.up * .5f, -Vector3.up * 100, Color.red);
            return true;
        }
        else
            return false;
    }

    private void ControllWallCollide(Vector3 _direction)
    {
        if (transform.position.y <= -.15f)
        {
            _myCollider.material = GameManager.Instance.slideMaterial;
            transform.DOMove(transform.position + _direction, .25f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
        }
    }

    public void StopPlayer()
    {
        runningspeed = 0;
        crown.SetActive(false);
    }

    public void ResetMaterial()
    {
        _myCollider.material = null;
    }

    public IEnumerator Swimming()
    {
        yield return new WaitForSeconds(.5f);
        _swimPos = transform.position;
        _swimPos.y = -2.15f;
        playerAnimator.SetTrigger("Swim");
        transform.DOMove(_swimPos, 1f).OnComplete(() => {
            _rb.isKinematic = true;
            GameManager.Instance.RemovePlayer(this);
            Destroy(this);
        });
    }

    public void GetName(string name)
    {
        nameText.text = name;
    }

    // ============================ *** END
}