using UnityEngine;
using System.Collections;

public class AnimateKing : MonoBehaviour
{
    public float Timer = 10f;
    public AnimationClip EnrageAnimClip;
    public AudioClip EnrageClip;

    private Animator _animator;
    private float _maxTimer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _maxTimer = Timer;
        Timer = 3.5f;
    }

    private void Update()
    {
        Timer -= Time.deltaTime;

        if (Timer <= 0f)
        {
            StartCoroutine(StarAnim(EnrageAnimClip.length));
            Timer = _maxTimer;
        }
    }

    private IEnumerator StarAnim(float animTime)
    {
       _animator.SetBool("Enraged", true);
        audio.PlayOneShot(EnrageClip);

        yield return new WaitForSeconds(animTime);

        _animator.SetBool("Enraged", false);
    }
}
