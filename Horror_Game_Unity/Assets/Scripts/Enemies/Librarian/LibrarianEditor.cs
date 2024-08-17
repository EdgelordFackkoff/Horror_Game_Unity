using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Librarian))]
public class LibrarianEditor : Editor
{
    private void OnSceneGUI()
    {
        Librarian libfov = (Librarian)target;
        Handles.color = Color.white;

        Handles.DrawWireArc(libfov.transform.position, Vector3.up, Vector3.forward, 360, libfov.aggro_radius);
        
        Vector3 angleA = libfov.DirFromAngle(-libfov.aggro_angle / 2, false);
        Vector3 angleB = libfov.DirFromAngle(libfov.aggro_angle / 2, false);

        Handles.DrawLine(libfov.transform.position, libfov.transform.position + angleA * libfov.aggro_radius);
        Handles.DrawLine(libfov.transform.position, libfov.transform.position + angleB * libfov.aggro_radius);
    }
}
