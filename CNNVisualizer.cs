//using Unity.Sentis;
//using UnityEngine;
//using TMPro;

//public class CNNVisualizer : MonoBehaviour
//{
//    [Header("Model")]
//    public ModelAsset modelAsset;
//    public Texture2D inputImage;

//    [Header("Layout")]
//    public float cubeSize = 0.08f;
//    public float layerSpacing = 15f;
//    public float featureMapSpacing = 4f;
//    public float cubeGap = 0.01f;

//    [Header("UI")]
//    public TMPro.TextMeshProUGUI predictionText;

//    private Worker worker;

//    void Start()
//    {
//        var model = ModelLoader.Load(modelAsset);
//        model.AddOutput("conv1_out", 10);
//        model.AddOutput("pool1_out", 15);
//        model.AddOutput("conv2_out", 25);
//        model.AddOutput("pool2_out", 30);

//        worker = new Worker(model, BackendType.CPU);
//        RunInference();
//    }

//    void RunInference()
//    {
//        var inputTensor = TextureToTensor(inputImage);
//        worker.Schedule(inputTensor);

//        var conv1 = (worker.PeekOutput("conv1_out") as Tensor<float>).ReadbackAndClone();
//        var pool1 = (worker.PeekOutput("pool1_out") as Tensor<float>).ReadbackAndClone();
//        var conv2 = (worker.PeekOutput("conv2_out") as Tensor<float>).ReadbackAndClone();
//        var pool2 = (worker.PeekOutput("pool2_out") as Tensor<float>).ReadbackAndClone();
//        var output = (worker.PeekOutput("output") as Tensor<float>).ReadbackAndClone();

//        float z = 0f;
//        float gap = 3f;  // gap between layers

//        // Each layer is centered at y=0 by offsetting based on grid height
//        SpawnInputLayer(inputImage, new Vector3(0, 0, z));

//        z += gap;
//        SpawnFeatureMaps("Conv1_ReLU", conv1, 8, 28, 28, new Vector3(0, 0, z));

//        z += gap + 8 * 0.15f;  // extra for feature map depth
//        SpawnFeatureMaps("Pool1", pool1, 8, 14, 14, new Vector3(0, 0, z));

//        z += gap + 8 * 0.15f;
//        SpawnFeatureMaps("Conv2_ReLU", conv2, 16, 14, 14, new Vector3(0, 0, z));

//        z += gap + 16 * 0.15f;
//        SpawnFeatureMaps("Pool2", pool2, 16, 7, 7, new Vector3(0, 0, z));

//        z += gap + 16 * 0.15f;
//        SpawnFlattenLayer(pool2, new Vector3(0, 0, z));

//        z += gap;
//        SpawnOutputLayer(output, new Vector3(0, 0, z));

//        int predicted = 0;
//        float maxVal = float.MinValue;
//        for (int i = 0; i < 10; i++)
//            if (output[0, i] > maxVal) { maxVal = output[0, i]; predicted = i; }
//        Debug.Log($"Predicted digit: {predicted}");

//        conv1.Dispose(); pool1.Dispose(); conv2.Dispose(); pool2.Dispose(); output.Dispose(); inputTensor.Dispose();
//    }
//    void SpawnInputLayer(Texture2D tex, Vector3 origin)
//    {
//        Transform parent = new GameObject("Layer_Input_1x28x28").transform;
//        parent.position = origin;

//        Color[] pixels = tex.GetPixels();
//        float step = cubeSize + cubeGap;
//        float halfSize = (28 * step) / 2f;

//        for (int y = 0; y < 28; y++)
//        {
//            for (int x = 0; x < 28; x++)
//            {
//                int flippedY = 27 - y;
//                float brightness = pixels[flippedY * 28 + x].grayscale;
//                Vector3 pos = origin + new Vector3(x * step - halfSize, -y * step + halfSize, 0);
//                SpawnCube(pos, brightness, parent);
//            }
//        }
//    }

//    void SpawnFeatureMaps(string layerName, Tensor<float> tensor, int channels, int height, int width, Vector3 origin)
//    {
//        Transform parent = new GameObject($"Layer_{layerName}_{channels}x{height}x{width}").transform;
//        parent.position = origin;

//        float step = cubeSize + cubeGap;

//        // Center the grid vertically
//        float halfHeight = (height * step) / 2f;
//        float halfWidth = (width * step) / 2f;

//        float minVal = float.MaxValue;
//        float maxVal = float.MinValue;
//        for (int c = 0; c < channels; c++)
//            for (int y = 0; y < height; y++)
//                for (int x = 0; x < width; x++)
//                {
//                    float val = tensor[0, c, y, x];
//                    if (val < minVal) minVal = val;
//                    if (val > maxVal) maxVal = val;
//                }

//        float range = maxVal - minVal;
//        if (range < 0.001f) range = 1f;

//        float mapOffset = 0.15f; // how much each feature map offsets in Z

//        for (int c = 0; c < channels; c++)
//        {
//            Transform fmParent = new GameObject($"FeatureMap_{c}").transform;
//            fmParent.parent = parent;

//            for (int y = 0; y < height; y++)
//            {
//                for (int x = 0; x < width; x++)
//                {
//                    float val = tensor[0, c, y, x];
//                    float brightness = (val - minVal) / range;

//                    Vector3 pos = origin + new Vector3(
//                        x * step - halfWidth,        // center horizontally
//                        -y * step + halfHeight,      // center vertically
//                        c * mapOffset                 // stack maps tightly in Z
//                    );

//                    SpawnCube(pos, brightness, fmParent);
//                }
//            }
//        }
//    }

//    void SpawnFlattenLayer(Tensor<float> pool2Tensor, Vector3 origin)
//    {
//        Transform parent = new GameObject("Layer_Flatten_784").transform;
//        parent.position = origin;

//        float step = cubeSize + cubeGap;

//        float minVal = float.MaxValue;
//        float maxVal = float.MinValue;
//        for (int c = 0; c < 16; c++)
//            for (int y = 0; y < 7; y++)
//                for (int x = 0; x < 7; x++)
//                {
//                    float val = pool2Tensor[0, c, y, x];
//                    if (val < minVal) minVal = val;
//                    if (val > maxVal) maxVal = val;
//                }

//        float range = maxVal - minVal;
//        if (range < 0.001f) range = 1f;

//        int columns = 28;
//        int idx = 0;
//        float halfWidth = (columns * step) / 2f;
//        int rows = 784 / columns; // 28
//        float halfHeight = (rows * step) / 2f;

//        for (int c = 0; c < 16; c++)
//            for (int y = 0; y < 7; y++)
//                for (int x = 0; x < 7; x++)
//                {
//                    float val = pool2Tensor[0, c, y, x];
//                    float brightness = (val - minVal) / range;

//                    int col = idx % columns;
//                    int row = idx / columns;

//                    Vector3 pos = origin + new Vector3(
//                        col * step - halfWidth,
//                        -row * step + halfHeight,
//                        0
//                    );

//                    SpawnCube(pos, brightness, parent);
//                    idx++;
//                }
//    }

//    void SpawnOutputLayer(Tensor<float> output, Vector3 origin)
//    {
//        Transform parent = new GameObject("Layer_Output_10").transform;
//        parent.position = origin;

//        float[] probs = new float[10];
//        float max = float.MinValue;
//        int predicted = 0;
//        for (int i = 0; i < 10; i++)
//        {
//            if (output[0, i] > max) { max = output[0, i]; predicted = i; }
//        }

//        float sum = 0;
//        for (int i = 0; i < 10; i++)
//        {
//            probs[i] = Mathf.Exp(output[0, i] - max);
//            sum += probs[i];
//        }
//        for (int i = 0; i < 10; i++)
//            probs[i] /= sum;

//        float step = cubeSize * 2.5f;

//        for (int i = 0; i < 10; i++)
//        {
//            Vector3 pos = origin + new Vector3(i * step, 0, 0);
//            GameObject cube = SpawnCube(pos, probs[i], parent);

//            Renderer rend = cube.GetComponent<Renderer>();
//            if (i == predicted)
//            {
//                // Bright green glow for prediction
//                rend.material.color = new Color(0.2f, 1f, 0.4f);
//                cube.transform.localScale = Vector3.one * cubeSize * 2.5f;
//            }
//            else
//            {
//                // Dim cubes use the probability for brightness
//                float b = probs[i] * 0.3f;
//                rend.material.color = new Color(b, b, b + 0.05f); // slight blue tint
//            }
//        }

//        if (predictionText != null)
//            predictionText.text = $"Predicted: {predicted}   Confidence: {probs[predicted] * 100:F1}%";
//    }

//    GameObject SpawnCube(Vector3 position, float brightness, Transform parent)
//    {
//        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        cube.transform.position = position;
//        cube.transform.localScale = Vector3.one * cubeSize;
//        cube.transform.parent = parent;

//        Destroy(cube.GetComponent<Collider>());

//        Renderer rend = cube.GetComponent<Renderer>();
//        rend.material = new Material(Shader.Find("Unlit/Color"));

//        // Color gradient: dark blue (inactive) → cyan → white (highly active)
//        Color color;
//        if (brightness < 0.5f)
//        {
//            // Dark blue to cyan
//            float t = brightness / 0.5f;
//            color = Color.Lerp(
//                new Color(0.02f, 0.02f, 0.08f),   // near black with blue tint
//                new Color(0.0f, 0.6f, 0.8f),       // cyan
//                t
//            );
//        }
//        else
//        {
//            // Cyan to white
//            float t = (brightness - 0.5f) / 0.5f;
//            color = Color.Lerp(
//                new Color(0.0f, 0.6f, 0.8f),       // cyan
//                new Color(1.0f, 1.0f, 1.0f),       // white
//                t
//            );
//        }

//        rend.material.color = color;
//        return cube;
//    }

//    void CreateLabel(string text, Vector3 position, Transform parent)
//    {
//        GameObject labelObj = new GameObject($"Label_{text}");
//        labelObj.transform.position = position;
//        labelObj.transform.parent = parent;

//        var tm = labelObj.AddComponent<TextMesh>();
//        tm.text = text;
//        tm.characterSize = 0.15f;
//        tm.fontSize = 50;
//        tm.anchor = TextAnchor.MiddleCenter;
//        tm.color = Color.white;
//    }

//    Tensor<float> TextureToTensor(Texture2D tex)
//    {
//        var tensor = new Tensor<float>(new TensorShape(1, 1, 28, 28));
//        Color[] pixels = tex.GetPixels();

//        for (int y = 0; y < 28; y++)
//            for (int x = 0; x < 28; x++)
//            {
//                int flippedY = 27 - y;
//                float val = pixels[flippedY * 28 + x].grayscale;
//                val = (val - 0.1307f) / 0.3081f;
//                tensor[0, 0, y, x] = val;
//            }

//        return tensor;
//    }

//    void OnDestroy()
//    {
//        worker?.Dispose();
//    }


//    // Add this method to CNNVisualizer
//    public void UpdateVisualization(Texture2D newInput)
//    {
//        // Destroy all old cubes
//        ClearAllLayers();

//        // Re-run with new input
//        inputImage = newInput;
//        RunInference();
//    }

//    void ClearAllLayers()
//    {
//        string[] layerNames = {
//        "Layer_Input_1x28x28",
//        "Layer_Conv1_ReLU_8x28x28",
//        "Layer_Pool1_8x14x14",
//        "Layer_Conv2_ReLU_16x14x14",
//        "Layer_Pool2_16x7x7",
//        "Layer_Flatten_784",
//        "Layer_Output_10"
//    };

//        foreach (string name in layerNames)
//        {
//            GameObject obj = GameObject.Find(name);
//            if (obj != null)
//                Destroy(obj);
//        }
//    }
//}

using Unity.Sentis;
using UnityEngine;
using TMPro;

public class CNNVisualizer : MonoBehaviour
{
    [Header("Model")]
    public ModelAsset modelAsset;
    public Texture2D inputImage;

    [Header("Layout")]
    public float cubeSize = 0.08f;
    public float layerSpacing = 15f;
    public float featureMapSpacing = 4f;
    public float cubeGap = 0.01f;

    [Header("UI")]
    public TextMeshProUGUI predictionText;

    private Worker worker;

    // Store renderers for each layer so we can update colors without respawning
    private Renderer[] inputRenderers;
    private Renderer[] conv1Renderers;
    private Renderer[] pool1Renderers;
    private Renderer[] conv2Renderers;
    private Renderer[] pool2Renderers;
    private Renderer[] flattenRenderers;
    private Renderer[] outputRenderers;
    private GameObject[] outputCubes;

    void Start()
    {
        var model = ModelLoader.Load(modelAsset);
        model.AddOutput("conv1_out", 10);
        model.AddOutput("pool1_out", 15);
        model.AddOutput("conv2_out", 25);
        model.AddOutput("pool2_out", 30);

        worker = new Worker(model, BackendType.CPU);

        // Spawn all cubes once
        SpawnAllLayers();

        // Run initial inference with blank or default image
        if (inputImage != null)
            RunInference(inputImage);
    }

    //void SpawnAllLayers()
    //{
    //    float z = 0f;
    //    float gap = 3f;

    //    // Input: 1x28x28 = 784 cubes
    //    inputRenderers = SpawnGrid("Layer_Input", 28, 28, new Vector3(0, 0, z));

    //    z += gap;
    //    // Conv1: 8x28x28 = 6272 cubes
    //    conv1Renderers = SpawnFeatureMapGrid("Layer_Conv1_ReLU", 8, 28, 28, new Vector3(0, 0, z));

    //    z += gap + 8 * 0.15f;
    //    // Pool1: 8x14x14 = 1568 cubes
    //    pool1Renderers = SpawnFeatureMapGrid("Layer_Pool1", 8, 14, 14, new Vector3(0, 0, z));

    //    z += gap + 8 * 0.15f;
    //    // Conv2: 16x14x14 = 3136 cubes
    //    conv2Renderers = SpawnFeatureMapGrid("Layer_Conv2_ReLU", 16, 14, 14, new Vector3(0, 0, z));

    //    z += gap + 16 * 0.15f;
    //    // Pool2: 16x7x7 = 784 cubes
    //    pool2Renderers = SpawnFeatureMapGrid("Layer_Pool2", 16, 7, 7, new Vector3(0, 0, z));

    //    z += gap + 16 * 0.15f;
    //    // Flatten: 784 cubes in 28x28 grid
    //    flattenRenderers = SpawnGrid("Layer_Flatten", 28, 28, new Vector3(0, 0, z));

    //    z += gap;
    //    // Output: 10 cubes
    //    SpawnOutputGrid(new Vector3(0, 0, z));
    //}

    void SpawnAllLayers()
    {
        float z = 0f;

        inputRenderers = SpawnGrid("Layer_Input", 28, 28, new Vector3(0, 0, z));

        z += layerSpacing;
        conv1Renderers = SpawnFeatureMapGrid("Layer_Conv1_ReLU", 8, 28, 28, new Vector3(0, 0, z));

        z += layerSpacing + 8 * featureMapSpacing;
        pool1Renderers = SpawnFeatureMapGrid("Layer_Pool1", 8, 14, 14, new Vector3(0, 0, z));

        z += layerSpacing + 8 * featureMapSpacing;
        conv2Renderers = SpawnFeatureMapGrid("Layer_Conv2_ReLU", 16, 14, 14, new Vector3(0, 0, z));

        z += layerSpacing + 16 * featureMapSpacing;
        pool2Renderers = SpawnFeatureMapGrid("Layer_Pool2", 16, 7, 7, new Vector3(0, 0, z));

        z += layerSpacing + 16 * featureMapSpacing;
        flattenRenderers = SpawnGrid("Layer_Flatten", 28, 28, new Vector3(0, 0, z));

        z += layerSpacing;
        SpawnOutputGrid(new Vector3(0, 0, z));
    }

    // Spawn a simple 2D grid of cubes (for input and flatten layers)
    Renderer[] SpawnGrid(string name, int cols, int rows, Vector3 origin)
    {
        Transform parent = new GameObject(name).transform;
        parent.position = origin;

        float step = cubeSize + cubeGap;
        float halfW = (cols * step) / 2f;
        float halfH = (rows * step) / 2f;

        Renderer[] renderers = new Renderer[cols * rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 pos = origin + new Vector3(x * step - halfW, -y * step + halfH, 0);
                renderers[y * cols + x] = SpawnCube(pos, parent);
            }
        }
        return renderers;
    }

    // Spawn feature map grids (for conv and pool layers)
    //Renderer[] SpawnFeatureMapGrid(string name, int channels, int height, int width, Vector3 origin)
    //{
    //    Transform parent = new GameObject(name).transform;
    //    parent.position = origin;

    //    float step = cubeSize + cubeGap;
    //    float halfW = (width * step) / 2f;
    //    float halfH = (height * step) / 2f;
    //    float mapOffset = 0.15f;

    //    Renderer[] renderers = new Renderer[channels * height * width];

    //    for (int c = 0; c < channels; c++)
    //    {
    //        Transform fmParent = new GameObject($"FeatureMap_{c}").transform;
    //        fmParent.parent = parent;

    //        for (int y = 0; y < height; y++)
    //        {
    //            for (int x = 0; x < width; x++)
    //            {
    //                Vector3 pos = origin + new Vector3(
    //                    x * step - halfW,
    //                    -y * step + halfH,
    //                    c * mapOffset
    //                );

    //                int idx = c * (height * width) + y * width + x;
    //                renderers[idx] = SpawnCube(pos, fmParent);
    //            }
    //        }
    //    }
    //    return renderers;
    //}

    Renderer[] SpawnFeatureMapGrid(string name, int channels, int height, int width, Vector3 origin)
    {
        Transform parent = new GameObject(name).transform;
        parent.position = origin;

        float step = cubeSize + cubeGap;
        float halfW = (width * step) / 2f;
        float halfH = (height * step) / 2f;

        Renderer[] renderers = new Renderer[channels * height * width];

        for (int c = 0; c < channels; c++)
        {
            Transform fmParent = new GameObject($"FeatureMap_{c}").transform;
            fmParent.parent = parent;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 pos = origin + new Vector3(
                        x * step - halfW,
                        -y * step + halfH,
                        c * featureMapSpacing  // uses your Inspector value now
                    );

                    int idx = c * (height * width) + y * width + x;
                    renderers[idx] = SpawnCube(pos, fmParent);
                }
            }
        }
        return renderers;
    }

    void SpawnOutputGrid(Vector3 origin)
    {
        Transform parent = new GameObject("Layer_Output").transform;
        parent.position = origin;

        float step = cubeSize * 2.5f;
        outputRenderers = new Renderer[10];
        outputCubes = new GameObject[10];

        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = origin + new Vector3(i * step, 0, 0);
            GameObject cube = CreateCube(pos, parent);
            outputRenderers[i] = cube.GetComponent<Renderer>();
            outputCubes[i] = cube;
        }
    }

    // --- INFERENCE AND COLOR UPDATE ---

    public void RunInference(Texture2D tex)
    {
        var inputTensor = TextureToTensor(tex);
        worker.Schedule(inputTensor);

        var conv1 = (worker.PeekOutput("conv1_out") as Tensor<float>).ReadbackAndClone();
        var pool1 = (worker.PeekOutput("pool1_out") as Tensor<float>).ReadbackAndClone();
        var conv2 = (worker.PeekOutput("conv2_out") as Tensor<float>).ReadbackAndClone();
        var pool2 = (worker.PeekOutput("pool2_out") as Tensor<float>).ReadbackAndClone();
        var output = (worker.PeekOutput("output") as Tensor<float>).ReadbackAndClone();

        // Update input layer colors from texture
        UpdateInputColors(tex);

        // Update feature map colors
        UpdateFeatureMapColors(conv1Renderers, conv1, 8, 28, 28);
        UpdateFeatureMapColors(pool1Renderers, pool1, 8, 14, 14);
        UpdateFeatureMapColors(conv2Renderers, conv2, 16, 14, 14);
        UpdateFeatureMapColors(pool2Renderers, pool2, 16, 7, 7);

        // Update flatten colors (reuse pool2 data flattened)
        UpdateFlattenColors(pool2);

        // Update output colors
        UpdateOutputColors(output);

        conv1.Dispose(); pool1.Dispose(); conv2.Dispose(); pool2.Dispose();
        output.Dispose(); inputTensor.Dispose();
    }

    void UpdateInputColors(Texture2D tex)
    {
        Color[] pixels = tex.GetPixels();
        for (int y = 0; y < 28; y++)
        {
            for (int x = 0; x < 28; x++)
            {
                int flippedY = 27 - y;
                float brightness = pixels[flippedY * 28 + x].grayscale;
                int idx = y * 28 + x;
                if (idx < inputRenderers.Length)
                    SetCubeColor(inputRenderers[idx], brightness);
            }
        }
    }

    void UpdateFeatureMapColors(Renderer[] renderers, Tensor<float> tensor, int channels, int height, int width)
    {
        // Find min/max for normalization
        float minVal = float.MaxValue;
        float maxVal = float.MinValue;
        for (int c = 0; c < channels; c++)
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    float val = tensor[0, c, y, x];
                    if (val < minVal) minVal = val;
                    if (val > maxVal) maxVal = val;
                }

        float range = maxVal - minVal;
        if (range < 0.001f) range = 1f;

        for (int c = 0; c < channels; c++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float val = tensor[0, c, y, x];
                    float brightness = (val - minVal) / range;
                    int idx = c * (height * width) + y * width + x;
                    if (idx < renderers.Length)
                        SetCubeColor(renderers[idx], brightness);
                }
            }
        }
    }

    void UpdateFlattenColors(Tensor<float> pool2Tensor)
    {
        float minVal = float.MaxValue;
        float maxVal = float.MinValue;
        for (int c = 0; c < 16; c++)
            for (int y = 0; y < 7; y++)
                for (int x = 0; x < 7; x++)
                {
                    float val = pool2Tensor[0, c, y, x];
                    if (val < minVal) minVal = val;
                    if (val > maxVal) maxVal = val;
                }

        float range = maxVal - minVal;
        if (range < 0.001f) range = 1f;

        int idx = 0;
        for (int c = 0; c < 16; c++)
            for (int y = 0; y < 7; y++)
                for (int x = 0; x < 7; x++)
                {
                    float val = pool2Tensor[0, c, y, x];
                    float brightness = (val - minVal) / range;
                    if (idx < flattenRenderers.Length)
                        SetCubeColor(flattenRenderers[idx], brightness);
                    idx++;
                }
    }

    void UpdateOutputColors(Tensor<float> output)
    {
        float[] probs = new float[10];
        float max = float.MinValue;
        int predicted = 0;
        for (int i = 0; i < 10; i++)
        {
            if (output[0, i] > max) { max = output[0, i]; predicted = i; }
        }

        float sum = 0;
        for (int i = 0; i < 10; i++)
        {
            probs[i] = Mathf.Exp(output[0, i] - max);
            sum += probs[i];
        }
        for (int i = 0; i < 10; i++)
            probs[i] /= sum;

        for (int i = 0; i < 10; i++)
        {
            if (i == predicted)
            {
                outputRenderers[i].material.color = new Color(0.2f, 1f, 0.4f);
                outputCubes[i].transform.localScale = Vector3.one * cubeSize * 2.5f;
            }
            else
            {
                float b = probs[i] * 0.3f;
                outputRenderers[i].material.color = new Color(b, b, b + 0.05f);
                outputCubes[i].transform.localScale = Vector3.one * cubeSize * 1.5f;
            }
        }

        if (predictionText != null)
            predictionText.text = $"Predicted: {predicted}   Confidence: {probs[predicted] * 100:F1}%";
    }

    // --- CORE HELPERS ---

    void SetCubeColor(Renderer rend, float brightness)
    {
        Color color;
        if (brightness < 0.5f)
        {
            float t = brightness / 0.5f;
            color = Color.Lerp(
                new Color(0.02f, 0.02f, 0.08f),
                new Color(0.0f, 0.6f, 0.8f),
                t
            );
        }
        else
        {
            float t = (brightness - 0.5f) / 0.5f;
            color = Color.Lerp(
                new Color(0.0f, 0.6f, 0.8f),
                new Color(1.0f, 1.0f, 1.0f),
                t
            );
        }
        rend.material.color = color;
    }

    Renderer SpawnCube(Vector3 position, Transform parent)
    {
        GameObject cube = CreateCube(position, parent);
        Renderer rend = cube.GetComponent<Renderer>();
        rend.material.color = new Color(0.02f, 0.02f, 0.08f); // start dark
        return rend;
    }

    GameObject CreateCube(Vector3 position, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = Vector3.one * cubeSize;
        cube.transform.parent = parent;
        Destroy(cube.GetComponent<Collider>());

        Renderer rend = cube.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Unlit/Color"));
        return cube;
    }

    Tensor<float> TextureToTensor(Texture2D tex)
    {
        var tensor = new Tensor<float>(new TensorShape(1, 1, 28, 28));
        Color[] pixels = tex.GetPixels();

        for (int y = 0; y < 28; y++)
            for (int x = 0; x < 28; x++)
            {
                int flippedY = 27 - y;
                float val = pixels[flippedY * 28 + x].grayscale;
                val = (val - 0.1307f) / 0.3081f;
                tensor[0, 0, y, x] = val;
            }
        return tensor;
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
