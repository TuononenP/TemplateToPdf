import {
    Edit,
    SimpleForm,
    TextInput,
    required,
    TopToolbar,
    Button,
    SaveButton,
    DeleteButton,
    Toolbar,
    useTranslate,
    useNotify,
    useInput,
    useRedirect
} from 'react-admin';
import { Link } from 'react-router-dom';
import ArrowBack from '@mui/icons-material/ArrowBack';
import { Box } from '@mui/material';
import AceEditor from 'react-ace';

// Import ace editor themes and modes
import 'ace-builds/webpack-resolver';
import 'ace-builds/src-min-noconflict/ext-language_tools';
import 'ace-builds/src-min-noconflict/mode-handlebars';
import 'ace-builds/src-min-noconflict/theme-twilight';
import 'ace-builds/src-min-noconflict/theme-xcode';

const EditActions = () => (
    <TopToolbar>
        <Button
            component={Link}
            to="/templates"
            label="Back"
            startIcon={<ArrowBack />}
        />
    </TopToolbar>
);

const EditToolbar = () => (
    <Toolbar>
        <Box flex={1} display="flex" gap={1}>
            <SaveButton />
        </Box>
        <DeleteButton
            mutationMode="pessimistic"
            confirmTitle="Delete template"
            confirmContent="Are you sure you want to delete this template?"
        />
    </Toolbar>
);

const ContentInput = ({ source, ...rest }: any) => {
    const translate = useTranslate();
    const theme = localStorage.getItem('theme') || 'light';
    const {
        field: { value, onChange, onBlur }
    } = useInput({
        source,
        validate: [required()]
    });
    
    return (
        <Box mt={2}>
            <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                {translate('templates.fields.content')} *
            </Box>
            <AceEditor
                mode="handlebars"
                theme={theme === 'dark' ? 'twilight' : 'xcode'}
                name={source}
                width="1000px"
                height="500px"
                fontSize={14}
                showPrintMargin={false}
                showGutter={true}
                highlightActiveLine={true}
                value={value || ''}
                onChange={(newValue) => {
                    onChange(newValue);
                }}
                onLoad={(editor) => {
                    editor.session.setUseWrapMode(true);
                }}
                onBlur={onBlur}
                setOptions={{
                    enableBasicAutocompletion: true,
                    enableLiveAutocompletion: true,
                    enableSnippets: true,
                    showLineNumbers: true,
                    tabSize: 4,
                    useWorker: false
                }}
            />
        </Box>
    );
};

export const TemplateEdit = () => {
    const notify = useNotify();
    const redirect = useRedirect();
    
    return (
        <Edit 
            actions={<EditActions />}
            mutationMode="optimistic"
            mutationOptions={{
                onSuccess: () => {
                    notify('templates.notifications.updated', { type: 'success' });
                    redirect('list', 'templates');
                },
                onError: () => {
                    notify('templates.notifications.error.update', { type: 'error' });
                }
            }}
        >
            <SimpleForm toolbar={<EditToolbar />}>
                <TextInput source="name" validate={[required()]} fullWidth />
                <ContentInput source="content" validate={[required()]} />
            </SimpleForm>
        </Edit>
    );
}; 