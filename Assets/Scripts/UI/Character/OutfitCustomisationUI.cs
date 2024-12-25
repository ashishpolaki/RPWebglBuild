using CharacterCustomisation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OutfitCustomisationUI : MonoBehaviour
{
    [SerializeField] private Transform scrollParent;
    [SerializeField] private OutfitStyleUI outfitPRefab;
    [SerializeField] private TextMeshProUGUI outfitNameText;

    private OutfitType outfitType;
    private Dictionary<OutfitType, List<OutfitStyleUI>> outfitStylesDictionary = new Dictionary<OutfitType, List<OutfitStyleUI>>();

    [Space(10), Header("Capture Outfit Settings")]
    [SerializeField] private Vector3 outfitCaptureCharacterPosition;
    [SerializeField] private CaptureOutfitTextureSettings[] captureTextureSettings;

    private Character character;
    private Character outfitCaptureCharacter;
    private bool isCharacterLoaded;

    private void DisableCurrentOutfits()
    {
        if (outfitStylesDictionary.ContainsKey(outfitType))
        {
            foreach (var item in outfitStylesDictionary[outfitType])
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    private void LoadCharacter(OutfitType _outfitType)
    {
        if (!isCharacterLoaded)
        {
            isCharacterLoaded = true;
            character = UGSManager.Instance.PlayerData.character;
            outfitCaptureCharacter = CharacterCustomisationManager.Instance.InstantiateCharacter();
            outfitCaptureCharacter.GetComponent<Animator>().CrossFade("CaptureCharacter", 0);
            outfitCaptureCharacter.transform.position = outfitCaptureCharacterPosition;
            outfitCaptureCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);

            //If Character Doesnt have any outfit applied, apply the default first outfit 
            if(character.CurrentTorso == -1 || character.CurrentHips == -1)
            {
                character.ChangeUpperOutfit(CharacterCustomisationManager.Instance.GetUpperOutfitEconomy(0));
                character.ChangeLowerOutfit(CharacterCustomisationManager.Instance.GetLowerOutfitEconomy(0));
            }
        }
        //Set Transform
        if (outfitCaptureCharacter != null)
        {
            if (_outfitType == OutfitType.Upper)
            {
                outfitCaptureCharacter.EnableUpperBody();
            }
            else if (_outfitType == OutfitType.Lower)
            {
                outfitCaptureCharacter.EnableLowerBody();
            }
        }
    }

    private CaptureOutfitTextureSettings GetCaptureTextureSettings(OutfitType outfitType)
    {
        foreach (var part in captureTextureSettings)
        {
            if (part.outfitType == outfitType)
            {
                return part;
            }
        }
        return new CaptureOutfitTextureSettings();
    }

    IEnumerator IESpawnUpperOutfits()
    {
        List<OutfitStyleUI> partStyleUIList = new List<OutfitStyleUI>();
        foreach (var item in CharacterCustomisationManager.Instance.UpperOutfits)
        {
            //Capture Styles in UI with the character parts
            CaptureOutfitTextureSettings captureOutfitTextureSettings = GetCaptureTextureSettings(outfitType);
            outfitCaptureCharacter.ChangeUpperOutfit(CharacterCustomisationManager.Instance.GetUpperOutfitEconomy(item.index));
            yield return null;
            RenderTexture renderTexture = GameManager.Instance.CaptureObject.CaptureWithCustom(outfitCaptureCharacter.gameObject, captureOutfitTextureSettings.Offset, captureOutfitTextureSettings.FieldOfView, captureOutfitTextureSettings.RenderTextureSize);

            OutfitStyleUI outfitStyle = Instantiate(outfitPRefab, scrollParent);
            outfitStyle.SetData(this, renderTexture, item.index, outfitType);
            partStyleUIList.Add(outfitStyle);
        }
        outfitStylesDictionary.Add(outfitType, partStyleUIList);
    }

    IEnumerator IESpawnLowerOutfits()
    {
        List<OutfitStyleUI> partStyleUIList = new List<OutfitStyleUI>();
        foreach (var item in CharacterCustomisationManager.Instance.LowerOutfits)
        {
            //Capture Styles in UI with the character parts
            CaptureOutfitTextureSettings captureOutfitTextureSettings = GetCaptureTextureSettings(outfitType);
            outfitCaptureCharacter.ChangeLowerOutfit(CharacterCustomisationManager.Instance.GetLowerOutfitEconomy(item.index));
            yield return null;
            RenderTexture renderTexture = GameManager.Instance.CaptureObject.CaptureWithCustom(outfitCaptureCharacter.gameObject, captureOutfitTextureSettings.Offset, captureOutfitTextureSettings.FieldOfView, captureOutfitTextureSettings.RenderTextureSize);

            OutfitStyleUI outfitStyle = Instantiate(outfitPRefab, scrollParent);
            outfitStyle.SetData(this, renderTexture, item.index, outfitType);
            partStyleUIList.Add(outfitStyle);
        }
        outfitStylesDictionary.Add(outfitType, partStyleUIList);
    }

    public void SetOutfitType(OutfitType _outfitType)
    {
        LoadCharacter(_outfitType);
        DisableCurrentOutfits();

        outfitType = _outfitType;

        if (outfitType == OutfitType.Upper)
        {
            outfitNameText.text = "Upper Outfit";
            if (!outfitStylesDictionary.ContainsKey(outfitType))
            {
              StartCoroutine(IESpawnUpperOutfits());
            }
            else
            {
                //Enable the stored outfits
                foreach (var item in outfitStylesDictionary[outfitType])
                {
                    item.gameObject.SetActive(true);
                }
            }
        }
        else if (outfitType == OutfitType.Lower)
        {
            outfitNameText.text = "Lower Outfit";

            if (!outfitStylesDictionary.ContainsKey(outfitType))
            {
                StartCoroutine(IESpawnLowerOutfits());
            }
            else
            {
                //Enable the stored outfits
                foreach (var item in outfitStylesDictionary[outfitType])
                {
                    item.gameObject.SetActive(true);
                }
            }
        }

    }

    public void SetOutfit(int partIndex, OutfitType outfitType)
    {
        //Deselect all the outfits
        foreach (var item in outfitStylesDictionary[outfitType])
        {
            item.UnSelect();
        }

        if (outfitType == OutfitType.Lower)
        {
            character.ChangeLowerOutfit(CharacterCustomisationManager.Instance.GetLowerOutfitEconomy(partIndex));
        }
        else if (outfitType == OutfitType.Upper)
        {
            character.ChangeUpperOutfit(CharacterCustomisationManager.Instance.GetUpperOutfitEconomy(partIndex));
        }
    }

}
