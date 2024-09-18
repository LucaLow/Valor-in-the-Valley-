using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class BuildingHealth : Health
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }
    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);

    }

    private void Update() {

        if (IsDead()) {
            if (gameObject.tag == "Town Hall")
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SceneManager.LoadScene("Main Menu");
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }

}