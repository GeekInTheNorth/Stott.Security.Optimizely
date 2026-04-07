import { createContext, useState, useCallback } from "react";
import PropTypes from 'prop-types';
import axios from 'axios';

const StottSecurityContext = createContext();

export const StottSecurityProvider = ({ children, ...props }) => {

    const [permissionPolicySettings, setPermissionPolicySettings] = useState({ isEnabled: false, isInherited: false });
    const [permissionPolicyCollection, setDirectiveCollection] = useState([]);
    const [permissionPolicySourceFilter, setPermissionPolicySourceFilter] = useState('');
    const [permissionPolicyDirectiveFilter, setPermissionPolicyDirectiveFilter] = useState('AllEnabled');
    const [permissionPolicyDirectivesInherited, setPermissionPolicyDirectivesInherited] = useState(false);

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    // Debounce function
    const debounce = (func, delay) => {
        let debounceTimer;
        return function(...args) {
            const context = this;
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => func.apply(context, args), delay);
        };
    };

    const getPermissionPolicyDirectives = (appId, hostName) => {
        getFilteredDirectives(permissionPolicySourceFilter, permissionPolicyDirectiveFilter, appId, hostName);
    };

    const getFilteredDirectives = useCallback(
        debounce(async (sourceName, directiveName, appId, hostName) => {
            await axios.get(import.meta.env.VITE_PERMISSION_POLICY_SOURCE_LIST, { params: { sourceFilter: sourceName, enabledFilter: directiveName, appId: appId, hostName: hostName } })
                .then((response) => {
                    if (Array.isArray(response.data)){
                        setDirectiveCollection(response.data);
                    }
                    else{
                        handleShowFailureToast("Get Permissions Policy Directives", "Failed to retrieve Permissions Policy Directives.");
                    }
                },
                () => {
                    handleShowFailureToast("Error", "Failed to retrieve the Permissions Policy Directives.");
                });
        }, 500),
        []
    );

    const getPermissionPolicySettings = async (appId, hostName) => {
        await axios.get(import.meta.env.VITE_PERMISSION_POLICY_SETTINGS_LOAD, { params: { appId: appId, hostName: hostName } })
            .then((response) => {
                setPermissionPolicySettings(response.data);
                setPermissionPolicyDirectivesInherited(response.data.isInherited);
            },
            () => {
                handleShowFailureToast("Error", "Failed to retrieve the Permissions Policy Settings.");
            });
    };

    const savePermissionPolicySettings = async (isEnabled, appId, hostName) => {
        await axios.post(import.meta.env.VITE_PERMISSION_POLICY_SETTINGS_SAVE, { isEnabled: isEnabled, appId: appId, hostName: hostName })
            .then(() => {
                handleShowSuccessToast("Success", "Permissions Policy Settings have been successfully saved.");
                getPermissionPolicySettings(appId, hostName);
                getPermissionPolicyDirectives(appId, hostName);
            },
            () => {
                handleShowFailureToast("Error", "Failed to save the Permissions Policy Settings.");
            });
    };

    const createPermissionPolicyOverride = async (appId, hostName) => {
        await axios.post(import.meta.env.VITE_PERMISSION_POLICY_OVERRIDE_CREATE, null, { params: { appId: appId, hostName: hostName } })
            .then(() => {
                handleShowSuccessToast("Success", "Permissions Policy settings and directives have been copied for override.");
                getPermissionPolicyDirectives(appId, hostName);
                getPermissionPolicySettings(appId, hostName);
            },
            () => {
                handleShowFailureToast("Error", "Failed to create Permissions Policy override.");
            });
    };

    const deletePermissionPolicyDirectives = async (appId, hostName) => {
        await axios.delete(import.meta.env.VITE_PERMISSION_POLICY_OVERRIDE_DELETE, { params: { appId: appId, hostName: hostName } })
            .then(() => {
                handleShowSuccessToast("Success", "Permissions Policy has been reverted to inherited.");
                getPermissionPolicyDirectives(appId, hostName);
                getPermissionPolicySettings(appId, hostName);
            },
            () => {
                handleShowFailureToast("Error", "Failed to revert Permissions Policy.");
            });
    };

    return (
        <StottSecurityContext.Provider value={
            {
                permissionPolicyCollection,
                permissionPolicySourceFilter,
                permissionPolicyDirectiveFilter,
                permissionPolicySettings,
                permissionPolicyDirectivesInherited,
                setPermissionPolicySourceFilter,
                setPermissionPolicyDirectiveFilter,
                getPermissionPolicyDirectives,
                getPermissionPolicySettings,
                savePermissionPolicySettings,
                createPermissionPolicyOverride,
                deletePermissionPolicyDirectives
            }}>
            {children}
        </StottSecurityContext.Provider>
    )
}

StottSecurityProvider.propTypes = {
    children: PropTypes.node.isRequired,
    showToastNotificationEvent: PropTypes.func
};

export { StottSecurityContext };
export default StottSecurityProvider;
