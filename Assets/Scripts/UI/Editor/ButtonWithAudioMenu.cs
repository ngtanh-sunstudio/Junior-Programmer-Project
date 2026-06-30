using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public static class ButtonWithAudioMenu
{
    private const string MenuPath = "GameObject/UI (Canvas)/Button With Audio";

    [MenuItem(MenuPath, false, 2032)]
    private static void CreateButtonWithAudio(MenuCommand menuCommand)
    {
        TMPro_CreateObjectMenu.AddButton(menuCommand);

        GameObject buttonObject = Selection.activeGameObject;
        Undo.AddComponent<UIButtonSoundEmitter>(buttonObject);
        Undo.SetCurrentGroupName("Create Button With Audio");
    }
}
