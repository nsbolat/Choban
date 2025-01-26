using UnityEngine;

public class TerrainTreeTransparency : MonoBehaviour
{
    public Transform player; // Oyuncu karakteri
    public Camera mainCamera; // Ana kamera
    public LayerMask transparencyLayer; // Şeffaflık uygulanacak layer
    public float transparentOpacity = 0.3f; // Şeffaflık seviyesi
    public float fadeSpeed = 2f; // Geçiş hızı

    private Material lastMaterial = null;
    private Color originalColor;

    void Update()
    {
        // Kamera ve oyuncu arasındaki çizgiyi kontrol et
        Vector3 direction = player.position - mainCamera.transform.position;
        Ray ray = new Ray(mainCamera.transform.position, direction.normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, direction.magnitude, transparencyLayer))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer != null)
            {
                Material material = renderer.material;

                if (material != lastMaterial) // Yeni bir obje ise
                {
                    ResetLastMaterial(); // Eski materyali eski haline döndür
                    lastMaterial = material;
                    originalColor = material.color;
                }

                // Şeffaflık uygula
                SetMaterialOpacity(material, transparentOpacity);
            }
        }
        else
        {
            ResetLastMaterial(); // Çarpmayan objeleri geri döndür
        }
    }

    private void ResetLastMaterial()
    {
        if (lastMaterial != null)
        {
            StartCoroutine(FadeTo(lastMaterial, originalColor.a));
            lastMaterial = null;
        }
    }

    private void SetMaterialOpacity(Material material, float targetOpacity)
    {
        StopAllCoroutines(); // Daha önceki fade işlemlerini durdur
        StartCoroutine(FadeTo(material, targetOpacity));
    }

    private System.Collections.IEnumerator FadeTo(Material material, float targetOpacity)
    {
        Color color = material.color;
        while (Mathf.Abs(color.a - targetOpacity) > 0.01f)
        {
            color.a = Mathf.Lerp(color.a, targetOpacity, Time.deltaTime * fadeSpeed);
            material.color = color;
            yield return null;
        }
        color.a = targetOpacity;
        material.color = color;
    }
}
