import {
    Edit,
    SimpleForm,
    TextInput,
    SelectInput,
    required,
    TopToolbar,
    Button,
    useTranslate,
    useRecordContext,
    useInput,
    useNotify,
    useRedirect,
} from 'react-admin';
import { useFormContext } from 'react-hook-form';
import { Link } from 'react-router-dom';
import ArrowBack from '@mui/icons-material/ArrowBack';
import AceEditor from 'react-ace';
import { Box, Typography, Paper } from '@mui/material';
import { useCallback } from 'react';
import { AssetType } from 'types';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';

// Import ace editor themes and modes
import 'ace-builds/webpack-resolver';
import 'ace-builds/src-min-noconflict/ext-language_tools';
import 'ace-builds/src-min-noconflict/mode-css';
import 'ace-builds/src-min-noconflict/mode-handlebars';
import 'ace-builds/src-min-noconflict/theme-twilight';
import 'ace-builds/src-min-noconflict/theme-xcode';

const EditActions = () => (
    <TopToolbar>
        <Button
            component={Link}
            to="/assets"
            label="Back"
            startIcon={<ArrowBack />}
        />
    </TopToolbar>
);

const ContentInput = ({ source, ...rest }: any) => {
    const translate = useTranslate();
    const record = useRecordContext();
    const theme = localStorage.getItem('theme') || 'light';
    const {
        field: { value, onChange, onBlur }
    } = useInput({
        source,
        validate: record?.type === AssetType.Image || record?.type === AssetType.Font ? undefined : [required()]
    });

    const mode = record?.type === AssetType.Css ? 'css' : 'handlebars';
    
    if (record?.type === AssetType.Image || record?.type === AssetType.Font) {
        return null;
    }

    return (
        <Box mt={2}>
            <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                {translate('assets.fields.content')} *
            </Box>
            <AceEditor
                mode={mode}
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

const BinaryContentInput = ({ source, ...rest }: any) => {
    const translate = useTranslate();
    const notify = useNotify();
    const record = useRecordContext();
    const form = useFormContext();
    const {
        field: { value, onChange }
    } = useInput({
        source,
        validate: record?.type === AssetType.Image || record?.type === AssetType.Font ? [required()] : undefined
    });

    const handleFileChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            // Check file size (10MB limit)
            if (file.size > 10 * 1024 * 1024) {
                notify('File size exceeds 10MB limit', { type: 'error' });
                return;
            }

            // Set the MIME type in the form
            form.setValue('mimeType', file.type);

            const reader = new FileReader();
            reader.onload = () => {
                const base64 = (reader.result as string).split(',')[1];
                onChange(base64);
            };
            reader.readAsDataURL(file);
        }
    }, [onChange, notify, form]);

    const handleDrop = useCallback((event: React.DragEvent<HTMLDivElement>) => {
        event.preventDefault();
        event.stopPropagation();
        
        const file = event.dataTransfer.files?.[0];
        if (file) {
            // Check file size (10MB limit)
            if (file.size > 10 * 1024 * 1024) {
                notify('File size exceeds 10MB limit', { type: 'error' });
                return;
            }

            // Set the MIME type in the form
            form.setValue('mimeType', file.type);

            const reader = new FileReader();
            reader.onload = () => {
                const base64 = (reader.result as string).split(',')[1];
                onChange(base64);
            };
            reader.readAsDataURL(file);
        }
    }, [onChange, notify, form]);

    const handleDragOver = useCallback((event: React.DragEvent<HTMLDivElement>) => {
        event.preventDefault();
        event.stopPropagation();
    }, []);

    if (record?.type !== AssetType.Image && record?.type !== AssetType.Font) {
        return null;
    }

    return (
        <Box mt={2}>
            <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                {translate('assets.fields.binaryContent')} *
            </Box>
            <Paper
                variant="outlined"
                sx={{
                    p: 2,
                    textAlign: 'center',
                    cursor: 'pointer',
                    bgcolor: theme => theme.palette.mode === 'dark' ? 'rgba(255, 255, 255, 0.05)' : 'rgba(0, 0, 0, 0.02)',
                    '&:hover': {
                        bgcolor: theme => theme.palette.mode === 'dark' ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.05)',
                    }
                }}
                onDrop={handleDrop}
                onDragOver={handleDragOver}
                onClick={() => document.getElementById('file-input')?.click()}
            >
                <input
                    id="file-input"
                    type="file"
                    accept={record?.type === AssetType.Image ? 'image/*' : '.woff2,.ttf,.otf'}
                    onChange={handleFileChange}
                    style={{ display: 'none' }}
                />
                <CloudUploadIcon sx={{ fontSize: 48, mb: 1, color: 'primary.main' }} />
                <Typography variant="body1" gutterBottom>
                    {translate('ra.input.file.upload_single')}
                </Typography>
                <Typography variant="body2" color="textSecondary">
                    {record?.type === AssetType.Image ? 'Supported formats: JPG, PNG, GIF, WebP' : 'Supported formats: WOFF2, TTF, OTF'}
                </Typography>
                <Typography variant="body2" color="textSecondary">
                    Maximum size: 10MB
                </Typography>
            </Paper>
            {value && (
                <Box mt={2}>
                    {record?.type === AssetType.Image && (
                        <img
                            src={`data:${form.getValues('mimeType')};base64,${value}`}
                            alt="Preview"
                            style={{ maxWidth: '100%', maxHeight: '300px', objectFit: 'contain' }}
                        />
                    )}
                    {record?.type === AssetType.Font && (
                        <Typography color="success.main">
                            {translate('assets.edit.fontUploaded')}
                        </Typography>
                    )}
                </Box>
            )}
        </Box>
    );
};

export const AssetEdit = () => {
    const notify = useNotify();
    const redirect = useRedirect();

    const transform = (data: any) => {
        const transformed = { ...data };
        
        // Ensure type is converted to number
        transformed.type = parseInt(transformed.type);
        
        // Clear content or binaryContent based on asset type
        if (transformed.type === AssetType.Image || transformed.type === AssetType.Font) {
            transformed.content = null;
        } else {
            transformed.binaryContent = null;
            transformed.mimeType = null;
        }
        
        return transformed;
    };

    return (
        <Edit 
            actions={<EditActions />}
            mutationMode="pessimistic"
            transform={transform}
            mutationOptions={{
                onSuccess: () => {
                    notify('assets.notifications.updated', { type: 'success' });
                    redirect('list', 'assets');
                },
                onError: () => {
                    notify('assets.notifications.error.update', { type: 'error' });
                }
            }}
        >
            <SimpleForm>
                <TextInput source="name" validate={[required()]} fullWidth />
                <TextInput source="referenceName" validate={[required()]} fullWidth />
                <SelectInput
                    source="type"
                    choices={[
                        { id: AssetType.Image, name: 'Image' },
                        { id: AssetType.Css, name: 'CSS' },
                        { id: AssetType.Font, name: 'Font' },
                        { id: AssetType.PartialTemplate, name: 'Partial Template' },
                    ]}
                    disabled
                />
                <TextInput source="description" multiline fullWidth />
                <ContentInput source="content" />
                <BinaryContentInput source="binaryContent" />
            </SimpleForm>
        </Edit>
    );
}; 