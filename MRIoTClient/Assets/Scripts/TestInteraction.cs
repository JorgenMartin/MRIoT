#nullable enable

using UnityEngine;

public class TestInteraction : MonoBehaviour
{
    public void firstHoverEntered()
    {
        Debug.Log("firstHoverEntered triggered");
        GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(0.5f, 0.5f, 0.5f);
    }

    public void lastHoverExited()
    {
        Debug.Log("lastGoverExited triggered");
        GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(0.6f, 0.6f, 0.6f);
    }

    public void hoverEntered()
    {
        Debug.Log("hoverEntered triggered");
        GetComponent<MeshRenderer>().material.color = Color.magenta;
    }

    public void hoverExited()
    {
        Debug.Log("hoverExited triggered");
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void firstSelectEntered()
    {
        Debug.Log("firstSelectEntered triggered");
        GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(0.7f, 0.7f, 0.7f);
    }

    public void lastSelectExited()
    {
        Debug.Log("lastSelectExited triggered");
        GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(0.8f, 0.8f, 0.8f);
    }

    public void selectEntered()
    {
        Debug.Log("selectEntered triggered");
        GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    public void selectExited()
    {
        Debug.Log("selectExited triggered");
        GetComponent<MeshRenderer>().material.color = Color.green;
    }

    public void firstFocusEntered()
    {
        Debug.Log("firstFocusEntered triggered");
        GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(0.3f, 0.3f, 0.3f);
    }

    public void lastFocusExited()
    {
        Debug.Log("lastFocusExited triggered");
        GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(0.4f, 0.4f, 0.8f);
    }

    public void focusEntered()
    {
        Debug.Log("focusEntered triggered");
        GetComponent<MeshRenderer>().material.color = Color.cyan;
    }

    public void focusExited()
    {
        Debug.Log("focusExited triggered");
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void activated()
    {
        Debug.Log("activated triggered");
        GetComponent<MeshRenderer>().material.color = Color.white;
    }

    public void deactivated()
    {
        Debug.Log("deactivated triggered"); 
        GetComponent<MeshRenderer>().material.color = Color.black;
    }
}
