using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System;
using Unity.Services.Authentication;

public class AuthenticationManager : MonoBehaviour
{
    [HideInInspector]
    public bool isSignedIn = false;
    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    void Start()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            //Shows how to get a playerID
            Debug.Log($"PlayedID: {AuthenticationService.Instance.PlayerId}");

            //Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

            const string successMessage = "Sign in anonymously succeeded!";
            Debug.Log(successMessage);

        };
    }

    /// <summary>
    /// Função para executar quando clicar no botão
    /// </summary>
    [ContextMenu("Sign In")]
    public async void OnClickSignIn()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            UpdateVariables();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Sign in anonymously failed with error code: {ex.ErrorCode}");
            
        }
    }

    private void UpdateVariables()
    {
        isSignedIn = AuthenticationService.Instance.IsSignedIn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
