import { Menu as ReactAdminMenu } from 'react-admin';
import DashboardIcon from '@mui/icons-material/Dashboard';
import SettingsIcon from '@mui/icons-material/Settings';

export const Menu = () => (
    <ReactAdminMenu>
        <ReactAdminMenu.DashboardItem
            primaryText="Dashboard"
            leftIcon={<DashboardIcon />}
        />
        <ReactAdminMenu.ResourceItem
            name="templates"
        />
        <ReactAdminMenu.ResourceItem
            name="assets"
        />
        <ReactAdminMenu.Item
            to="/settings"
            primaryText="Settings"
            leftIcon={<SettingsIcon />}
        />
    </ReactAdminMenu>
); 