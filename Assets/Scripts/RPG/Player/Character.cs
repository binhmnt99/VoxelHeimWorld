using UnityEngine;
using UnityEngine.UI;

namespace Voxel
{
    public class Character : MonoBehaviour
    {
[SerializeField] float health = 25;
    //[SerializeField] GameObject hitVFX;
    //[SerializeField] GameObject ragdoll;
    private GameObject playerCanvas;
    private Slider slider;
    public Gradient gradient;
    private Image fill;

    Animator animator;
    void Start()
    {
        playerCanvas = GameObject.Find("PlayerHealthBar");
        slider = playerCanvas.GetComponent<Slider>();
        fill = playerCanvas.transform.GetChild(0).GetComponent<Image>();
        animator = GetComponent<Animator>();
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        //animator.SetTrigger("damage");
        //CameraShake.Instance.ShakeCamera(2f, 0.2f);
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //Instantiate(ragdoll, transform.position, transform.rotation);
        Destroy(this.gameObject);
        GameObject canvasObject = GameObject.Find("Canvas");
        canvasObject.transform.GetChild(1).gameObject.SetActive(true);
        GameObject playercanvasObject = GameObject.Find("PlayerCanvas");
        playercanvasObject.gameObject.SetActive(false);
        Time.timeScale = 0f;
        
    }
    public void HitVFX(Vector3 hitPosition)
    {
        //GameObject hit = Instantiate(hitVFX, hitPosition, Quaternion.identity);
        //Destroy(hit, 3f);

    }
    }
}

