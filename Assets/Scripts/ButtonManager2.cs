using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonManager2 : MonoBehaviour
{
    [SerializeField] private Image imageCooldown;
    [SerializeField] private TMPro.TextMeshProUGUI textSpellNumber;

    public GameObject Canvas;
    public GameObject map;
    private int currentSpellNumber;
    private int spellNumberMax;
    private bool isInCooldown = false;
    public float cooldownTime = 10.0f;
    private float cooldownTimer = 0.0f;
    private RectTransform objectRectTransform;
    // Start is called before the first frame update
    void Start()
    {
        imageCooldown.fillAmount = 0.0f;
        spellNumberMax = map.GetComponent<MapManager>().deleteGroundMax;
        textSpellNumber.text = Mathf.RoundToInt(spellNumberMax).ToString();
        objectRectTransform = Canvas.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        changeButtonPosition();

        currentSpellNumber = map.GetComponent<MapManager>().spellNumber;
        textSpellNumber.text = Mathf.RoundToInt(currentSpellNumber).ToString();
        if (currentSpellNumber<spellNumberMax && !isInCooldown)
        {
            isInCooldown = true;
            cooldownTimer = 0;
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        while (cooldownTimer <= cooldownTime)
        {
            cooldownTimer += Time.deltaTime;
            imageCooldown.fillAmount = cooldownTimer / cooldownTime;
            yield return null;
        }
        isInCooldown = false;
        imageCooldown.fillAmount = 0.0f;
        map.GetComponent<MapManager>().spellNumber++;
        yield return null;
    }

    private void changeButtonPosition()
    {
        imageCooldown.transform.position = new Vector3(objectRectTransform.rect.width * 17 / 18, 50 * objectRectTransform.rect.height / 400, 0);
        imageCooldown.GetComponent<RectTransform>().sizeDelta = new Vector2(100 * objectRectTransform.rect.width / 900, 100 * objectRectTransform.rect.width / 900);
        transform.position = new Vector3(objectRectTransform.rect.width * 17 / 18, 50 * objectRectTransform.rect.height / 400, 0);
        GetComponent<RectTransform>().sizeDelta = new Vector2(80 * objectRectTransform.rect.width / 900, 80 * objectRectTransform.rect.width / 900);
        textSpellNumber.transform.position = new Vector3(objectRectTransform.rect.width * 78/80,  objectRectTransform.rect.height *5/80, 0);
        textSpellNumber.fontSize = objectRectTransform.rect.width/40;
    }
}
