using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI), typeof(Animator))]
public class ScreenMessage : MonoBehaviour
{
    public static ScreenMessage instance = default;
    
    private TextMeshProUGUI textMeshPro = default;
    private Animator anim = default;

    private int hashShow = Animator.StringToHash("SHOW");

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
        textMeshPro = GetComponent<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
    }

    public void ShowMessage(string text, int sprite = 0)
    {
        textMeshPro.text = $"<size=200><sprite={sprite}></size> \n {text}";
        anim.SetBool(hashShow, true);
    }

    public void HideMessage()
    {
        anim.SetBool(hashShow, false);
    }
}
