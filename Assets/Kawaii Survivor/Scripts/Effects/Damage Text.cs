using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshPro damageText;

    public void Animate(string damage, bool isCriticalHit)//string damage, bool isCriticalHit
    {
        damageText.text = damage.ToString();
        damageText.color = isCriticalHit ? Color.yellow : Color.white;

        animator.Play("Animate");
    }
}
