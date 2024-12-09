using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CaptureObject : MonoBehaviour
{
    [SerializeField] private Camera captureCamera;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 rotation = new Vector3(0, 0, 0);
    [SerializeField] private GraphicsFormat colorFormat;
    [SerializeField] private GraphicsFormat depthStencilFormat;
    [SerializeField] private Vector2Int renderTextureSize;

    private void Awake()
    {
        GameManager.Instance.SetCaptureObject(this);
    }

    public RenderTexture Capture(GameObject gameObject)
    {
        captureCamera.gameObject.SetActive(true);
        captureCamera.transform.position = gameObject.transform.position + offset;
        captureCamera.transform.eulerAngles = rotation;
        RenderTexture renderTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, colorFormat, depthStencilFormat);
        captureCamera.targetTexture = renderTexture;
        captureCamera.Render();
        captureCamera.gameObject.SetActive(false);
        return renderTexture;
    }
}
