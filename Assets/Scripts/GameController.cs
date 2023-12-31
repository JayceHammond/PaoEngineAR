using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour {

    [SerializeField] private GameObject diceMode;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button currButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button tableButton;
    [SerializeField] private Button modeButton;
    [SerializeField] private GameObject pointer;
    [SerializeField] private GameObject tableModeController;
    [SerializeField] private GameObject diceChooser;
    [SerializeField] private GameObject tableButtons;
    [SerializeField] private GameObject sessionOrigin;
    [SerializeField] private GameObject resultView;
    [SerializeField] private GameObject popupGameObject;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;
    private ARTogglePlaneDetection arTogglePlaneDetection;

    private SwipeModeController swipeModeController;
    private FallingModeController fallingModeController;

    private List<GameObject> dice;
    private List<GameObject> presetInstantiatedDice;
    private List<Sprite> diceSprites;
    private List<Sprite> modeSprites;
    private List<Sprite> tableSprites;
    private Sprite presetSprite;
    private GameObject instantiatedDie;
    private Rigidbody rb;
    private CanvasGroup resultCanvasGroup;
    private TextMeshProUGUI textMeshPro;
    private Image previousButtonImage;
    private Image nextButtonImage;
    private Image currentButtonImage;
    private Image tableButtonImage;
    private Image throwButtonImage;
    private Popup popup;

    private int currentDie;
    private int tempResult = 0;

    private bool throwed = false;
    private bool throwable = true;

    void Start() {
        diceMode.SetActive(true);
        
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arTogglePlaneDetection = sessionOrigin.GetComponent<ARTogglePlaneDetection>();

        currentDie = 0;

        dice = Container.Instance.dice;
        diceSprites = Container.Instance.diceSprites;
        modeSprites = Container.Instance.gameModesSprites;
        tableSprites = Container.Instance.tableModeSprites;
        presetSprite = Container.Instance.presetSprite;

        textMeshPro = resultView.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        resultCanvasGroup = resultView.gameObject.GetComponent<CanvasGroup>();
        resultView.SetActive(true);
        LeanTween.alphaCanvas(resultCanvasGroup, 0f, 0f);

        throwButtonImage = modeButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        tableButtonImage = tableButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        previousButtonImage = prevButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        nextButtonImage = nextButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        currentButtonImage = currButton.transform.GetChild(0).gameObject.GetComponent<Image>();

        popup = popupGameObject.GetComponent<Popup>();
        
        swipeModeController = new SwipeModeController();
        fallingModeController = new FallingModeController();
        presetInstantiatedDice = new List<GameObject>();

        if (Container.Instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            SetupSwipeToThrow();
        } else if (Container.Instance.throwMode == ThrowMode.FALLING) {
            SetupFallingDices();
        }
    }
    
    void Update() {
        if(!Container.Instance.tableModeIsEnabled && !Container.Instance.tableConstraint) {
            if (Container.Instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                if (!swipeModeController.IsThrowed && swipeModeController.IsThrowable) {
                    swipeModeController.UpdateDiePosition();
                    tempResult = 0;
                } else if(swipeModeController.IsThrowed && !swipeModeController.IsThrowable) {
                    if(swipeModeController.PickAndSet()) {
                        StartCoroutine(DelayedThrowable());
                    }

                    CheckDieResult();
                }

                if (swipeModeController.IsThrowable) {
                    swipeModeController.SwipeDie();
                }
            } else if(Container.Instance.throwMode == ThrowMode.FALLING) {
                if(instantiatedDie != null || presetInstantiatedDice.Count > 0) {
                    CheckDieResult();
                }
            }
        } else if(Container.Instance.tableConstraint) {
            CheckDieResult();
        }

        if(Container.Instance.themeChanged) {
            SetDiceMaterial();
        }
    }

    private void SetupSwipeToThrow() {
        pointer.SetActive(false);
        
        if(instantiatedDie != null) {
            Destroy(instantiatedDie);
        }

        instantiatedDie = Instantiate(dice[currentDie]);
        swipeModeController.Die = instantiatedDie;
        swipeModeController.UpdateDiePosition();
        throwButtonImage.sprite = modeSprites[0];
    }
    
    private void SetupFallingDices() {
      /*  if (instantiatedDie != null) {
            Destroy(instantiatedDie);
        }*/
        
        pointer.SetActive(true);
        throwButtonImage.sprite = modeSprites[1];
    }

    public void OnThrowModeButton() {
        if(!Container.Instance.tableConstraint) {
            tempResult = 0;

            if (Container.Instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                Container.Instance.throwMode = ThrowMode.FALLING;

                currentButtonImage.sprite = diceSprites[currentDie];
                nextButtonImage.sprite = diceSprites[currentDie + 1];
                previousButtonImage.sprite = presetSprite;
                
                SetupFallingDices();
            } else {
                Container.Instance.throwMode = ThrowMode.SWIPE_TO_THROW;

                currentButtonImage.sprite = diceSprites[currentDie];
                nextButtonImage.sprite = diceSprites[currentDie + 1];
                previousButtonImage.sprite = diceSprites[dice.Count - 1];

                SetupSwipeToThrow();
            }
        }
    } 


    public void OnSideSwitchButton(int i) {
        if(i == 0) {
            currentDie--;

            if(currentDie == -1) {
                if(Container.Instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                    currentDie = dice.Count - 1;
                } else if(Container.Instance.throwMode == ThrowMode.FALLING) {
                    currentDie = dice.Count;
                }
            }
        } else if(i == 1) {
            currentDie++;

            if(currentDie == dice.Count) {
                if(Container.Instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                    currentDie = 0;
                }
            }

            if(currentDie == dice.Count + 1 && Container.Instance.throwMode == ThrowMode.FALLING) {
                currentDie = 0;
            }
        }


        if (Container.Instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            Destroy(instantiatedDie);
            instantiatedDie = Instantiate(dice[currentDie]);
            
            swipeModeController.Die = instantiatedDie;
            swipeModeController.IsThrowed = false;
            swipeModeController.IsThrowable = true;
            swipeModeController.UpdateDiePosition();
        }

        if(Container.Instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            if (currentDie == 0) {
                previousButtonImage.sprite = diceSprites[dice.Count - 1];
            } else {
                previousButtonImage.sprite = diceSprites[currentDie - 1];
            }

            currentButtonImage.sprite = diceSprites[currentDie];

            if (currentDie == dice.Count - 1) {
                nextButtonImage.sprite = diceSprites[0];
            } else {
                nextButtonImage.sprite = diceSprites[currentDie + 1];
            }
        } else if (Container.Instance.throwMode == ThrowMode.FALLING) {
            if (currentDie == 0) {
                previousButtonImage.sprite = presetSprite;
            } else {
                previousButtonImage.sprite = diceSprites[currentDie - 1];
            }

            if(currentDie == 6) {
                currentButtonImage.sprite = presetSprite;
                nextButtonImage.sprite = diceSprites[0];
                previousButtonImage.sprite = diceSprites[currentDie - 2];
            } else {
                currentButtonImage.sprite = diceSprites[currentDie];
            }

            if (currentDie == dice.Count - 1) {
                nextButtonImage.sprite = presetSprite;
            } else {
                nextButtonImage.sprite = diceSprites[currentDie + 1];
            }
        }
    }

    public void OnCurrentDieButton() {
        if (Container.Instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            swipeModeController.IsThrowed = false;
            swipeModeController.IsThrowable = true;
            swipeModeController.UpdateDiePosition();
        } else if (Container.Instance.throwMode == ThrowMode.FALLING) {
            tempResult = 0;

           /* if (instantiatedDie != null) {
                Destroy(instantiatedDie);
            }*/

            if (currentDie == dice.Count) {  // preset
                if(presetInstantiatedDice.Count > 0) {
                    for(int i = presetInstantiatedDice.Count - 1; i >= 0; i--) {
                        Destroy(presetInstantiatedDice[i]);
                        presetInstantiatedDice.RemoveAt(i);
                    }
                }

                presetInstantiatedDice = fallingModeController.DropPreset(Container.Instance.activePreset);
            } else {
                instantiatedDie = fallingModeController.DropDie(dice[currentDie]);
            }
        }
    }

    public void OnTableModeButton() {
        if (Container.Instance.tableModeIsEnabled) {
            tableModeController.SetActive(false);
            diceChooser.SetActive(true);
            tableButtons.SetActive(false);
            pointer.SetActive(false);
            modeButton.gameObject.SetActive(true);
            tableButtonImage.sprite = tableSprites[0];

            if(Container.Instance.tableConstraint) {
                Container.Instance.throwMode = ThrowMode.FALLING;
                SetupFallingDices();
            } else {
                if(Container.Instance.tableMeshes.Count == 1)
                    popup.ShowPopup(Container.Instance.errorDictionary["not_builded"]);

                for(int i = Container.Instance.tableMeshes.Count - 1; i >= 0; i--) {
                    Destroy(Container.Instance.tableMeshes[i]);
                    Container.Instance.tableMeshes.RemoveAt(i);
                }

                arTogglePlaneDetection.EnablePlaneDetection(true);

                if (Container.Instance.throwMode == ThrowMode.FALLING)
                    SetupFallingDices();
                else {
                    SetupSwipeToThrow();
                }
            }

            Container.Instance.tableModeIsEnabled = false;
        } else {
            Container.Instance.tableModeIsEnabled = true;
            tableButtonImage.sprite = tableSprites[1];

            if (instantiatedDie != null)
                Destroy(instantiatedDie);

            tableModeController.SetActive(true);
            diceChooser.SetActive(false);
            tableButtons.SetActive(true);
            pointer.SetActive(true);
            modeButton.gameObject.SetActive(false);

            arTogglePlaneDetection.EnablePlaneDetection(false);
        }
    }

    private void CheckDieResult() {
        if(Container.Instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            DieUtils dieResult = instantiatedDie.GetComponent<DieUtils>();

            if(dieResult.availableResult) {
                SetDieResult(dieResult);
            }
        } else if(Container.Instance.throwMode == ThrowMode.FALLING) {

            if(currentDie == dice.Count) {
                List<DieUtils> results = presetInstantiatedDice.Select(d => d.GetComponent<DieUtils>()).ToList();

                SetDiceResults(results);
            } else {
                DieUtils dieResult = instantiatedDie.GetComponent<DieUtils>();

                if(dieResult.availableResult) {
                    SetDieResult(dieResult);
                }
            }
        }
    }

    private void SetDieResult(DieUtils dieResult) {
        if(dieResult.result != tempResult) {
            resultView.SetActive(true);
            LeanTween.alphaCanvas(resultCanvasGroup, 1f, 0f);
            textMeshPro.SetText(dieResult.result.ToString());

            StartCoroutine(ShowResultView());
        }

        tempResult = dieResult.result;
    }

    private void SetDiceResults(List<DieUtils> results) {
        int totalResult = 0;
        bool available = false;

        foreach(DieUtils result in results) {
            totalResult += result.result;

            if(result.availableResult) {
                available = true;
            }
        }

        if(totalResult != tempResult && available) {
            resultView.SetActive(true);
            LeanTween.alphaCanvas(resultCanvasGroup, 1f, 0f);
            textMeshPro.SetText(totalResult.ToString());

            StartCoroutine(ShowResultView());
        }

        tempResult = totalResult;
    }

    private IEnumerator ShowResultView() {
        yield return new WaitForSeconds(3);
        LeanTween.alphaCanvas(resultCanvasGroup, 0f, 1f).setOnComplete(DisableResult);
    }

    private void DisableResult() {
        resultView.SetActive(false);
    }

    private void SetDiceMaterial() {
        Theme currTheme = Container.Instance.activeTheme;
        
        Container.Instance.themeDie.color = currTheme.GetDieColor();
        Container.Instance.themeNumber.color = currTheme.GetNumbColor();
        
        Container.Instance.themeChanged = false;
    }

    private IEnumerator DelayedThrowable() {
        yield return new WaitForSeconds(0.1f);
        swipeModeController.IsThrowable = true;
    }
}
