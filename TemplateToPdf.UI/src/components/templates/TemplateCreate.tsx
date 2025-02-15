import {
    Create,
    SimpleForm,
    TextInput,
    required,
    TopToolbar,
    Button,
    useTranslate,
    useNotify,
    useRedirect,
    useInput
} from 'react-admin';
import { Link } from 'react-router-dom';
import ArrowBack from '@mui/icons-material/ArrowBack';
import AceEditor from 'react-ace';
import { Box } from '@mui/material';

// Import ace editor themes and modes
import 'ace-builds/webpack-resolver';
import 'ace-builds/src-min-noconflict/ext-language_tools';
import 'ace-builds/src-min-noconflict/mode-handlebars';
import 'ace-builds/src-min-noconflict/theme-twilight';
import 'ace-builds/src-min-noconflict/theme-xcode';

const defaultTemplate = `<html>
    <head>
        <style>
            body { font-family: Arial, sans-serif; padding: 20px; }
            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            th, td { padding: 8px; text-align: left; border: 1px solid #ddd; }
            th { background-color: #f5f5f5; font-weight: bold; }
            h1, h2 { margin-bottom: 10px; }
        </style>
    </head>
    <body>
        <h1>{{title}}</h1>
        <div>{{content}}</div>
    </body>
</html>`;

const CreateActions = () => (
    <TopToolbar>
        <Button
            component={Link}
            to="/templates"
            label="Back"
            startIcon={<ArrowBack />}
        />
    </TopToolbar>
);

const ContentInput = ({ source, ...rest }: any) => {
    const translate = useTranslate();
    const theme = localStorage.getItem('theme') || 'light';
    const {
        field: { value, onChange, onBlur }
    } = useInput({
        source,
        defaultValue: defaultTemplate,
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

export const TemplateCreate = () => {
    const notify = useNotify();
    const redirect = useRedirect();
    
    return (
        <Create 
            actions={<CreateActions />}
            mutationOptions={{
                onSuccess: () => {
                    notify('templates.notifications.created', { type: 'success' });
                    redirect('list', 'templates');
                },
                onError: () => {
                    notify('templates.notifications.error.create', { type: 'error' });
                }
            }}
        >
            <SimpleForm>
                <TextInput source="name" validate={[required()]} fullWidth />
                <ContentInput source="content" validate={[required()]} />
            </SimpleForm>
        </Create>
    );
}; 