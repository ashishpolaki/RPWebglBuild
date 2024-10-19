using UnityEngine;

namespace HorseRace
{
    public class ActivateFinishLine : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private GameObject finishLineObj;
        [SerializeField] private GameObject horseGatesObj;
        [SerializeField] private GameObject navmeshObstacle;

        [SerializeField] private Vector3 changeFinishLinePosition;
        [SerializeField] private int controlPointIndex = 30;
        #endregion

        #region Private Variables
        private HorseController preWinnerHorse;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            EventManager.Instance.OnControlPointChangeEvent += HandleControlPointChange;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnControlPointChangeEvent -= HandleControlPointChange;
        }
        #endregion

        #region Subscribed Methods
        private void HandleControlPointChange(int HorseNumber, int _controlPointIndex)
        {
            if(preWinnerHorse == null)
            {
                return;
            }
            if (HorseNumber == preWinnerHorse.HorseNumber && _controlPointIndex == controlPointIndex)
            {
                ChangeFinishLinePosition();
            }
        }
        #endregion

        #region Public Methods
        public void ChangeFinishLinePosition()
        {
            finishLineObj.gameObject.SetActive(true);
            horseGatesObj.gameObject.SetActive(false);
            if(navmeshObstacle != null)
            {
                navmeshObstacle.gameObject.SetActive(false);
            }
            finishLineObj.transform.position = changeFinishLinePosition;
        }
        public void SetPreWinner(HorseController horseController)
        {
            preWinnerHorse = horseController;
        }
        #endregion
    }
}
