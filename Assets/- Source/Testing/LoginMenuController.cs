﻿using UnityEngine;
using System.Collections;
using Soomla.Profile;
using Soomla;
using GameSparks.Core;
using GameSparks.Api.Requests;
using System.Collections.Generic;


public class LoginMenuController : MonoBehaviour
{
    public GameObject m_OfflinePanel = null;
    public GameObject m_WaitingPanel = null;
    public GameObject m_OnlinePanel = null;
    public float m_GSConnectRetryTime = 2f;

    [HideInInspector]
    public static bool GSReconnecting = true;

    private bool m_ProfileReady = false;
    private bool m_UIHidden = false;
    protected float m_GSConnectRetryTimer = 0f;

    #region Unity Events
    void Start()
    {
		m_OnlinePanel.SetActive(false);
		m_WaitingPanel.SetActive(true);
		m_OfflinePanel.SetActive(false);

        ProfileManager.OnStateChanged += ProfileManager_OnStateChanged;
        GS.GameSparksAvailable += GS_OnStateChanged;
    }

    void OnDestroy()
    {
        ProfileManager.OnStateChanged -= ProfileManager_OnStateChanged;        
    }

    void Update()
    {
        if( GS.Available )
        {
            m_GSConnectRetryTimer = 0f;
            GSReconnecting = false;
        }
        else
        {
            m_GSConnectRetryTimer += Time.unscaledDeltaTime;

            if( m_GSConnectRetryTimer >= m_GSConnectRetryTime )
            {
                m_GSConnectRetryTimer = 0f;
                GS.Disconnect();
                GS.Reconnect();
            }
        }
    }
    #endregion

    #region GS Event Handlers
    protected void GS_OnStateChanged(bool isAvailable)
    {
        GSReconnecting = false;

        if( !isAvailable )
           GS_Reconnect_Cor();
		else
		{
			if (ProfileManager.IsLoggedIn) {
				m_OnlinePanel.SetActive(true);
				m_WaitingPanel.SetActive(false);
				m_OfflinePanel.SetActive(false);
			} 
			else
			{
				m_OnlinePanel.SetActive(false);
				m_WaitingPanel.SetActive(false);
				m_OfflinePanel.SetActive(true);

			}
		}
    }

    protected IEnumerable GS_Reconnect_Cor()
    {
        GSReconnecting = true;
        GS.Reconnect();
        yield return new WaitForSeconds(1);
    }
    
    #endregion

    #region Profile Event Handlers
    void ProfileManager_OnStateChanged(ProfileManager.ProviderState _state)
    {
        switch (_state)
        {
            case ProfileManager.ProviderState.LoggedIn:
                if( ProfileManager.CurrentProvider == Provider.FACEBOOK )
                {
                    new FacebookConnectRequest().SetAccessToken(FB.AccessToken).Send((response) =>
                    {
						if (response.HasErrors) 
						{
							ProfileManager.Logout();
                            Debug.LogError("[GS] Facebook auth error: " + response.Errors.JSON);
						}
						else 
						{
							m_OnlinePanel.SetActive(true);
							m_WaitingPanel.SetActive(false);
							m_OfflinePanel.SetActive(false);
						}
                    });
                }

                else if( ProfileManager.CurrentProvider == Provider.GOOGLE )
                {
                }

                else if( ProfileManager.CurrentProvider == Provider.TWITTER )
                {
                }

                break;

            case ProfileManager.ProviderState.LoggingIn:
                m_OnlinePanel.SetActive(false);
                m_WaitingPanel.SetActive(true);
                m_OfflinePanel.SetActive(false);
                break;

            case ProfileManager.ProviderState.LoggedOut:
                m_OnlinePanel.SetActive(false);
                m_WaitingPanel.SetActive(false);
                m_OfflinePanel.SetActive(true);
                break;

            case ProfileManager.ProviderState.LoggingOut:
                m_OnlinePanel.SetActive(false);
                m_WaitingPanel.SetActive(true);
                m_OfflinePanel.SetActive(false);
                break;
        }
    }
    #endregion

    #region Offline
    public void BtnFBLoginClicked()
    {
        ProfileManager.Login(Provider.FACEBOOK);
    }

    public void BtnGPLoginClicked()
    {
        ProfileManager.Login(Provider.GOOGLE);
    }

    public void BtnTWLoginClicked()
    {
        ProfileManager.Login(Provider.TWITTER);
    }
    #endregion

    #region Online
    public void BtnStatusClicked()
    {
        if (!FB.IsLoggedIn) return;

        SoomlaProfile.UpdateStatus(Provider.FACEBOOK, "This is a Treasure Chest test status!");
    }

    public void BtnPostClicked()
    {
        if (!FB.IsLoggedIn) return;

        SoomlaProfile.UpdateStory(Provider.FACEBOOK, "This is a Treasure Chest test message!", "Huzzah!", "Caption", "Description", "", "http://www.alansjourney.com/wp-content/uploads/2015/02/Oh-Hell-Yeah.png");
    }

    public void BtnFriendsClicked()
    {
        if (!FB.IsLoggedIn) return;

        SoomlaProfile.GetContacts(Provider.FACEBOOK, true);
    }

    public void BtnLogoutClicked()
    {
        if (!FB.IsLoggedIn) return;

        SoomlaProfile.Logout(Provider.FACEBOOK);
        m_OnlinePanel.SetActive(false);
        m_OfflinePanel.SetActive(true);
    }

    public void BtnToggleUI()
    {
        if (!FB.IsLoggedIn) return;

        m_UIHidden = !m_UIHidden;

        Transform onlinePanel = transform.FindChild("Online Panel");
        onlinePanel.FindChild("Status").gameObject.SetActive(!m_UIHidden);
        onlinePanel.FindChild("Post").gameObject.SetActive(!m_UIHidden);
        onlinePanel.FindChild("Logout").gameObject.SetActive(!m_UIHidden);

        string btnText = m_UIHidden ? "Show" : "Hide";
        onlinePanel.FindChild("Status").gameObject.SetActive(!m_UIHidden);
        onlinePanel.FindChild("Hide UI").FindChild("Text").GetComponent<UnityEngine.UI.Text>().text = btnText;
    }
    #endregion
}