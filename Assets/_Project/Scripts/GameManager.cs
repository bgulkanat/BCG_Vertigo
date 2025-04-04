using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DataHolder _data;
    public enum AttachmentType { Sight, Mag, Barrel, Tactical, Stock}
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private GameObject _attachmentsLowerPanel;
    private Transform _attachmentsHolder;
    public GameObject[] _newAttachments;
    [SerializeField] private GameObject _attachmentsItem;
    private int _sightCount, _magCount, _barrelCount, _tacticalCount, _stockCount, _maxCount;
    private Color[] _atachmentsColor;
    private Color[] _equipButtonColor;
    private Color[] _statsColor;
    private int _choosedSight, _choosedMag, _choosedBarrel, _choosedStock, _choosedTactical;
    [SerializeField] private GameObject[] _verticalButonFrames;
    [SerializeField] private GameObject[] _lowerButonFrames;
    [SerializeField] private GameObject[] _sights;
    [SerializeField] private GameObject[] _mags;
    [SerializeField] private GameObject[] _barrels;
    [SerializeField] private GameObject[] _stocks;
    [SerializeField] private GameObject[] _tacticals;
    [SerializeField] private Button _equipButton;
    [SerializeField] private TextMeshProUGUI _equipText, _currentPanel;
    [SerializeField] private RectMask2D _statsRectMask;
    private Vector4 _padding;
    private Image _equipButtonImage;
    [SerializeField] private Image _sightImage, _magImage, _barrelImage, _stockImage, _tacticalImage, _sightAttachImage, _magAttachImage, _barrelAttachImage, _stockAttachImage, _tacticalAttachImage;
    private void Awake()
    {
        GetAttachmentSizes();

        _lowerButonFrames = new GameObject[_maxCount];
        _attachmentsHolder = _attachmentsLowerPanel.transform;
        
        _padding = _statsRectMask.padding;
        _padding.y = 0;
        _statsRectMask.padding = _padding;
        _equipButtonImage = _equipButton.GetComponent<Image>();

        SetButtonColors();
        GetPlayerPrefs();
        CallActivatedParts();
    }
    void Start()
    {
        for (int i = 0; i <= _maxCount - 1; i++)
        {
           _newAttachments[i]  = Instantiate(_attachmentsItem, _attachmentsHolder);
           _newAttachments[i].transform.gameObject.GetComponent<Image>().color = _atachmentsColor[i];
        }
        SetAttachmentsList(_data.MagNames, _data.MagImages, _choosedMag, AttachmentType.Sight);
    }
    private void SetAttachmentsList(string[] names, Sprite[] images, int selectedIndex, AttachmentType type)
    {
        foreach (var attachment in _newAttachments)
        {
            attachment.SetActive(false);
        }
        var index = 0;
        foreach (var item in names)
        {
            GameObject firstChild = _newAttachments[index].transform.GetChild(1).gameObject;
            GameObject secondChild = _newAttachments[index].transform.GetChild(2).gameObject;
            GameObject frame = _newAttachments[index].transform.GetChild(3).gameObject;
            _lowerButonFrames[index] = frame;
            firstChild.GetComponent<TextMeshProUGUI>().text = names[index];
            secondChild.GetComponent<Image>().sprite = images[index];

            _newAttachments[index].SetActive(true);

            int currentIndex = index;
            _newAttachments[index].GetComponent<Button>().onClick.RemoveAllListeners();
            switch (type)
            {
                case AttachmentType.Sight:
                    _newAttachments[index].GetComponent<Button>().onClick.AddListener(() => PreviewAttachment(_sights, currentIndex, AttachmentType.Sight));
                    break;
                case AttachmentType.Mag:
                    _newAttachments[index].GetComponent<Button>().onClick.AddListener(() => PreviewAttachment(_mags, currentIndex, AttachmentType.Mag));
                    break;
                case AttachmentType.Barrel:
                    _newAttachments[index].GetComponent<Button>().onClick.AddListener(() => PreviewAttachment(_barrels, currentIndex, AttachmentType.Barrel));
                    break;
                case AttachmentType.Stock:
                    _newAttachments[index].GetComponent<Button>().onClick.AddListener(() => PreviewAttachment(_stocks, currentIndex, AttachmentType.Stock));
                    break;
                case AttachmentType.Tactical:
                    _newAttachments[index].GetComponent<Button>().onClick.AddListener(() => PreviewAttachment(_tacticals, currentIndex, AttachmentType.Tactical));
                    break;
            }
            // previewButton.GetComponent<Button>().onClick.AddListener(() => PurchasePopUp(currentIndex, ItemType._Tire, tirePrice));
            index++;
        }
        DeactivateFramesExcept(_lowerButonFrames ,_lowerButonFrames[selectedIndex]);
    }
    public void OnAttachmentButtonPressed(int buttonIndex)
    {
        GetPlayerPrefs();
        CallActivatedParts();
        DeactivateFramesExcept(_verticalButonFrames ,_verticalButonFrames[buttonIndex]);
        _currentPanel.text = "ATTACHMENTS";
        _padding.y = 158;
        _statsRectMask.padding = _padding;
        _equipButton.gameObject.SetActive(true);
        _equipButton.enabled = false;
        _equipText.text = "EQUIPPED";
        _equipButtonImage.color = _equipButtonColor[1];
        _cameraHolder.GetComponent<CameraOrbit>().enabled = false;
        switch (buttonIndex)
        {
            case 0:
                SetAttachmentsList(_data.SightNames, _data.SightImages, _choosedSight, AttachmentType.Sight);
                break;
            case 1:
                SetAttachmentsList(_data.MagNames, _data.MagImages, _choosedMag, AttachmentType.Mag);
                break;
            case 2:
                SetAttachmentsList(_data.BarrelNames, _data.BarrelImages, _choosedBarrel, AttachmentType.Barrel);
                break;
            case 3:
                SetAttachmentsList(_data.StockNames, _data.StockImages, _choosedStock, AttachmentType.Stock);
                break;
            case 4:
                SetAttachmentsList(_data.TacticalNames, _data.TacticalImages, _choosedTactical, AttachmentType.Tactical);
                break;
            default:
                Debug.LogWarning("Geçersiz buton indeksi!");
                break;
        }
        _cameraHolder.GetComponent<CameraOrbit>().enabled = true;
    }
    private void DeactivateFramesExcept(GameObject[] array, GameObject activeFrame)
    {
        foreach (var obj in array)
        {
            obj.SetActive(obj == activeFrame);
        }
    }
    private void ActivateGunPart(GameObject[] array, int count)
    {
        foreach (var obj in array)
        {
            obj.SetActive(obj == array[count]);
        }
    }
    private void PreviewAttachment(GameObject[] array, int index, AttachmentType type)
    {
        print("tıklandı");
        ActivateGunPart(array, index);
        DeactivateFramesExcept(_lowerButonFrames, _lowerButonFrames[index]);
        GetPlayerPrefs();
        _equipButton.enabled = false;
        _equipButtonImage.color = _equipButtonColor[1];
        _equipText.text = "EQUIPPED";
        _equipButton.onClick.RemoveAllListeners();
        switch (type)
        {
            case AttachmentType.Sight:
                if (_choosedSight != index)
                {
                    _equipButton.enabled = true;
                    _equipButtonImage.color = _equipButtonColor[0];
                    _equipText.text = "EQUIP";
                    _equipButton.onClick.AddListener(() => OnClickEquip(index, AttachmentType.Sight));
                }
                break;
            case AttachmentType.Mag:
                if (_choosedMag != index)
                {
                    _equipButton.enabled = true;
                    _equipButtonImage.color = _equipButtonColor[0];
                    _equipText.text = "EQUIP";
                    _equipButton.onClick.AddListener(() => OnClickEquip(index, AttachmentType.Mag));
                }
                break;
            case AttachmentType.Barrel:
                if (_choosedBarrel != index)
                {
                    _equipButton.enabled = true;
                    _equipButtonImage.color = _equipButtonColor[0];
                    _equipText.text = "EQUIP";
                    _equipButton.onClick.AddListener(() => OnClickEquip(index, AttachmentType.Barrel));
                }
                break;
            case AttachmentType.Stock:
                if (_choosedStock != index)
                {
                    _equipButton.enabled = true;
                    _equipButtonImage.color = _equipButtonColor[0];
                    _equipText.text = "EQUIP";
                    _equipButton.onClick.AddListener(() => OnClickEquip(index, AttachmentType.Stock));
                }
                break;
            case AttachmentType.Tactical:
                if (_choosedTactical != index)
                {
                    _equipButton.enabled = true;
                    _equipButtonImage.color = _equipButtonColor[0];
                    _equipText.text = "EQUIP";
                    _equipButton.onClick.AddListener(() => OnClickEquip(index, AttachmentType.Tactical));
                }
                break;
        }
    }
    private void CallActivatedParts()
    {
        ActivateGunPart(_sights, _choosedSight);
        ActivateGunPart(_mags, _choosedMag);
        ActivateGunPart(_barrels, _choosedBarrel);
        ActivateGunPart(_stocks, _choosedStock);
        ActivateGunPart(_tacticals, _choosedTactical);
    }
    private void GetPlayerPrefs()
    {
        _choosedSight = PlayerPrefs.GetInt("Dash_Sight", 0);
        _choosedMag = PlayerPrefs.GetInt("Dash_Mag", 0);
        _choosedBarrel = PlayerPrefs.GetInt("Dash_Barrel", 0);
        _choosedStock = PlayerPrefs.GetInt("Dash_Stock", 0);
        _choosedTactical = PlayerPrefs.GetInt("Dash_Tactical", 0);
        SetPanelImages(_choosedSight, _choosedMag, _choosedBarrel, _choosedStock, _choosedTactical);
    }
    private void SetPanelImages(int sight, int mag, int barrel, int stock, int tactical)
    {
        _sightImage.sprite = _data.SightImages[sight];
        _magImage.sprite = _data.MagImages[mag];
        _barrelImage.sprite = _data.BarrelImages[barrel];
        _stockImage.sprite = _data.StockImages[stock];
        _tacticalImage.sprite = _data.TacticalImages[tactical];
        _sightAttachImage.sprite = _data.SightImages[sight];
        _magAttachImage.sprite = _data.MagImages[mag];
        _barrelAttachImage.sprite = _data.BarrelImages[barrel];
        _stockAttachImage.sprite = _data.StockImages[stock];
        _tacticalAttachImage.sprite = _data.TacticalImages[tactical];
        
    }
    private void SetButtonColors()
    {
        _atachmentsColor = new Color[_maxCount];
        _equipButtonColor = new Color[2];
        _statsColor = new Color[3];
        _atachmentsColor[0] = new Color32(0x9a, 0xc8, 0xca, 255);
        _atachmentsColor[1] = new Color32(0x26, 0xe0, 0x80, 255);
        _atachmentsColor[2] = new Color32(0x1c, 0xc4, 0xf8, 255);
        _atachmentsColor[3] = new Color32(0x9B, 0x1D, 0xF8, 255);
        _atachmentsColor[4] = new Color32(0xf8, 0x49, 0x1c, 255);
        _equipButtonColor[0] = new Color32(0x2E, 0x72, 0xC7, 255);
        _equipButtonColor[1] = new Color32(0x4f, 0x55, 0x75, 255);
        _statsColor[0] = new Color32(0x2E, 0x72, 0xC7, 255);
        _statsColor[1] = new Color32(0x26, 0x95, 0x78, 255);
        _statsColor[2] = new Color32(0xA9, 0x32, 0x32, 255);
    }
    private void GetAttachmentSizes()
    {
        _sightCount = _data.SightImages.Length;
        _magCount = _data.MagImages.Length;
        _barrelCount = _data.BarrelImages.Length;
        _tacticalCount = _data.TacticalImages.Length;
        _stockCount = _data.StockImages.Length;
        int[] counts = { _sightCount, _magCount, _barrelCount, _tacticalCount, _stockCount };
        _maxCount = counts.Max();
        _newAttachments = new GameObject[_maxCount];
    }
    private void OnClickEquip(int index, AttachmentType type)
    {
        switch (type)
        {
            case AttachmentType.Sight:
                _sightImage.sprite = _data.SightImages[index];
                _sightAttachImage.sprite = _data.SightImages[index];
                PlayerPrefs.SetInt("Dash_Sight", index);
                break;
            case AttachmentType.Mag:
                _magImage.sprite = _data.MagImages[index];
                _magAttachImage.sprite = _data.MagImages[index];
                PlayerPrefs.SetInt("Dash_Mag", index);
                break;
            case AttachmentType.Barrel:
                _barrelImage.sprite = _data.BarrelImages[index];
                _barrelAttachImage.sprite = _data.BarrelImages[index];
                PlayerPrefs.SetInt("Dash_Barrel", index);
                break;
            case AttachmentType.Stock:
                _stockImage.sprite = _data.StockImages[index];
                _stockAttachImage.sprite = _data.StockImages[index];
                PlayerPrefs.SetInt("Dash_Stock", index);
                break;
            case AttachmentType.Tactical:
                _tacticalImage.sprite = _data.TacticalImages[index];
                _tacticalAttachImage.sprite = _data.TacticalImages[index];
                PlayerPrefs.SetInt("Dash_Tactical", index);
                break;
        }
        _equipButton.enabled = false;
        _equipButtonImage.color = _equipButtonColor[1];
        _equipText.text = "EQUIPPED";
    }
    public void OnClickBack()
    {
        GetPlayerPrefs();
        CallActivatedParts();
        _currentPanel.text = "INVENTORY";
        _padding.y = 0;
        _statsRectMask.padding = _padding;
        _equipButton.gameObject.SetActive(false);
    }
}
