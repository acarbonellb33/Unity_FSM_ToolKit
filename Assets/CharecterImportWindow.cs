using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine.Animations;

public class CharacterImportWindow : UnityEditor.EditorWindow
{
    private bool importCharacter = false;
    private string characterGameObjectName = "";
    private GameObject characterModel;
    private List<AnimationClip> selectedAnimations = new List<AnimationClip>();
    private static List<AnimationClip> allAnimations = new List<AnimationClip>();
    
    [MenuItem("Window//FSM/Character Import Window")]
    public static void ShowWindow()
    {
        GetWindow<CharacterImportWindow>("Character Import");
        allAnimations = new List<AnimationClip>(Resources.FindObjectsOfTypeAll<AnimationClip>());
    }

    private void OnGUI()
    {
        GUILayout.Label("Character Import Settings", EditorStyles.boldLabel);
        
        // Toggle for importing character model
        importCharacter = EditorGUILayout.Toggle("Import Character Model", importCharacter);
        
        if (importCharacter)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Character GameObject Name");
            characterGameObjectName = EditorGUILayout.TextField(characterGameObjectName);
            EditorGUILayout.EndHorizontal();
            
            // Field for dragging model FBX
            characterModel = EditorGUILayout.ObjectField("Character Model (FBX)", characterModel, typeof(GameObject), false) as GameObject;
            
            // List of animations
            EditorGUILayout.LabelField("Animations to Import", EditorStyles.boldLabel);
            foreach (AnimationClip clip in allAnimations)
            {
                bool isSelected = selectedAnimations.Contains(clip);
                isSelected = EditorGUILayout.ToggleLeft(clip.name, isSelected);
                
                if (isSelected && !selectedAnimations.Contains(clip))
                {
                    selectedAnimations.Add(clip);
                }
                else if (!isSelected && selectedAnimations.Contains(clip))
                {
                    selectedAnimations.Remove(clip);
                }
            }

            if (GUILayout.Button("Import Character"))
            {
                ImportCharacter();
            }
        }
    }

    private void ImportCharacter()
    {
        if (string.IsNullOrEmpty(characterGameObjectName))
        {
            Debug.LogError("Character GameObject Name is empty!");
            return;
        }

        if (characterModel == null)
        {
            Debug.LogError("Character Model is not assigned!");
            return;
        }

        // Instantiate the character model
        GameObject instantiatedCharacter = Instantiate(characterModel);
        instantiatedCharacter.name = characterGameObjectName;

        // Create Animation Controller
        Animator animator = instantiatedCharacter.AddComponent<Animator>();
        AnimatorController controller = CreateAnimatorController();

        // Add animations to the controller
        foreach (AnimationClip clip in selectedAnimations)
        {
            AddAnimationToController(controller, clip);
        }

        // Set the default state to the first animation
        animator.runtimeAnimatorController = controller;
        animator.Play(selectedAnimations[0].name);

        Debug.Log("Character imported successfully!");
    }

    private AnimatorController CreateAnimatorController()
    {
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Character/Animations/CharacterController.controller");
        return controller;
    }

    private void AddAnimationToController(AnimatorController controller, AnimationClip clip)
    {
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
        AnimatorState state = rootStateMachine.AddState(clip.name);
        state.motion = clip;

        // Add a trigger for this animation
        controller.AddParameter(clip.name, AnimatorControllerParameterType.Trigger);

        // Set the default transition to this state
        AnimatorStateTransition defaultTransition = rootStateMachine.AddAnyStateTransition(state);
        defaultTransition.AddCondition(AnimatorConditionMode.If, 0, clip.name);
    }
}


