import { Menu as ReactAdminMenu } from 'react-admin';
import DescriptionIcon from '@mui/icons-material/Description';
import DashboardIcon from '@mui/icons-material/Dashboard';
import SettingsIcon from '@mui/icons-material/Settings';

export const Menu = () => (
    <ReactAdminMenu>
        <ReactAdminMenu.DashboardItem
            primaryText="Dashboard"
            leftIcon={<DashboardIcon />}
        />
        <ReactAdminMenu.Item
            to="/templates"
            primaryText="Templates"
            leftIcon={<DescriptionIcon />}
        />
        <ReactAdminMenu.Item
            to="/settings"
            primaryText="Settings"
            leftIcon={<SettingsIcon />}
        />
    </ReactAdminMenu>
); 