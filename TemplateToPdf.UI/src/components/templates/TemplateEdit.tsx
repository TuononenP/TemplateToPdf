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
import Handlebars from 'handlebars';
import { useState, useEffect, useCallback } from 'react';
import { registerHandlebarsHelpers } from '../../utils/handlebarsHelpers';

// Import ace editor themes and modes
import 'ace-builds/webpack-resolver';
import 'ace-builds/src-min-noconflict/ext-language_tools';
import 'ace-builds/src-min-noconflict/mode-handlebars';
import 'ace-builds/src-min-noconflict/mode-json';
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

const generateModelFromTemplate = (template: string): any => {
    const model: any = {};
    const regex = /{{[#/]?([^}]+)}}/g;
    const eachBlocksContent = new Map<string, Set<string>>();
    
    // First pass: collect all properties used within each blocks
    const eachRegex = /{{#each\s+([^}]+)}}([\s\S]*?){{\/each}}/g;
    let eachMatch;
    while ((eachMatch = eachRegex.exec(template)) !== null) {
        const arrayPath = eachMatch[1].trim();
        const blockContent = eachMatch[2];
        const properties = new Set<string>();
        
        let propMatch;
        const propRegex = /{{([^#/][^}]+)}}/g;
        while ((propMatch = propRegex.exec(blockContent)) !== null) {
            properties.add(propMatch[1].trim());
        }
        
        eachBlocksContent.set(arrayPath, properties);
    }

    // Second pass: process all expressions
    let match;
    while ((match = regex.exec(template)) !== null) {
        const path = match[1].trim();
        
        // Skip each/closing tags and helper functions
        if (path.startsWith('each ') || path.startsWith('/') || path.includes(' ') || path === 'each') {
            // Handle date fields referenced in formatDate
            if (path.startsWith('formatDate ')) {
                const parts = path.split(' ');
                if (parts.length > 1) {
                    const datePath = parts[1];
                    if (!getNestedValue(model, datePath)) {
                        setNestedValue(model, datePath, new Date().toISOString());
                    }
                }
            }
            continue;
        }

        // Handle nested paths
        const parts = path.split('.');
        let current = model;
        
        // If this property is part of an each block, skip it
        let isInEachBlock = false;
        for (const [, properties] of Array.from(eachBlocksContent.entries())) {
            if (properties.has(path)) {
                isInEachBlock = true;
                break;
            }
        }
        
        if (!isInEachBlock) {
            for (let i = 0; i < parts.length - 1; i++) {
                const part = parts[i];
                if (!(part in current)) {
                    current[part] = {};
                }
                current = current[part];
            }

            const lastPart = parts[parts.length - 1];
            if (!(lastPart in current)) {
                current[lastPart] = `Example ${lastPart}`;
            }
        }
    }

    // Create arrays for each blocks with example items
    for (const [arrayPath, properties] of Array.from(eachBlocksContent.entries())) {
        const parts = arrayPath.split('.');
        let current = model;
        
        // Create nested structure for array
        for (let i = 0; i < parts.length - 1; i++) {
            const part = parts[i];
            if (!(part in current)) {
                current[part] = {};
            }
            current = current[part];
        }

        const lastPart = parts[parts.length - 1];
        // Create example array with two items using the actual properties
        const item1: any = {};
        const item2: any = {};
        
        for (const prop of Array.from(properties)) {
            item1[prop] = `Example ${prop} 1`;
            item2[prop] = `Example ${prop} 2`;
        }

        current[lastPart] = [item1, item2];
    }

    return model;
};

const getNestedValue = (obj: any, path: string): any => {
    const parts = path.split('.');
    let current = obj;
    
    for (const part of parts) {
        if (current === undefined || current === null) return undefined;
        current = current[part];
    }
    
    return current;
};

const setNestedValue = (obj: any, path: string, value: any) => {
    const parts = path.split('.');
    let current = obj;
    
    for (let i = 0; i < parts.length - 1; i++) {
        const part = parts[i];
        if (!(part in current)) {
            current[part] = {};
        }
        current = current[part];
    }
    
    current[parts[parts.length - 1]] = value;
};

const ContentInput = ({ source, ...rest }: any) => {
    const translate = useTranslate();
    const theme = localStorage.getItem('theme') || 'light';
    const {
        field: { value, onChange, onBlur }
    } = useInput({
        source,
        validate: [required()]
    });
    
    const [preview, setPreview] = useState('');
    const [previewError, setPreviewError] = useState<string | null>(null);
    const [model, setModel] = useState(() => JSON.stringify({}, null, 2));
    const [modelError, setModelError] = useState<string | null>(null);
    const [helpersRegistered, setHelpersRegistered] = useState(false);

    // Update model when template changes
    useEffect(() => {
        if (value) {
            try {
                const generatedModel = generateModelFromTemplate(value);
                if (Object.keys(generatedModel).length > 0) {
                    setModel(JSON.stringify(generatedModel, null, 2));
                }
            } catch (error) {
                console.error('Error generating model:', error);
            }
        }
    }, [value]);

    // Register helpers and partials
    useEffect(() => {
        const initializeHelpers = async () => {
            try {
                await registerHandlebarsHelpers();
                setHelpersRegistered(true);
            } catch (error) {
                console.error('Error registering helpers:', error);
                setPreviewError('Error loading template helpers');
            }
        };
        
        initializeHelpers();
    }, []);

    const generatePreview = useCallback(() => {
        if (!helpersRegistered) {
            console.debug('Helpers not yet registered, skipping preview generation');
            return;
        }

        try {
            const template = Handlebars.compile(value || '');
            const parsedModel = JSON.parse(model);
            const rendered = template(parsedModel);
            setPreview(rendered);
            setPreviewError(null);
            setModelError(null);
        } catch (error) {
            if (error instanceof SyntaxError) {
                setModelError((error as Error).message);
            } else {
                setPreviewError((error as Error).message);
            }
            setPreview('');
        }
    }, [value, model, helpersRegistered]);

    useEffect(() => {
        generatePreview();
    }, [generatePreview]);
    
    return (
        <Box mt={2}>
            <Box display="flex" gap={4}>
                <Box flex={1}>
                    <Box>
                        <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                            {translate('templates.fields.content')} *
                        </Box>
                        <AceEditor
                            mode="handlebars"
                            theme={theme === 'dark' ? 'twilight' : 'xcode'}
                            name={source}
                            width="1000px"
                            height="400px"
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
                    <Box mt={2}>
                        <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                            {translate('templates.fields.modelForPreview')}
                        </Box>
                        {modelError && (
                            <Box color="error.main" fontSize="0.875rem" mb={1}>
                                {modelError}
                            </Box>
                        )}
                        <AceEditor
                            mode="json"
                            theme={theme === 'dark' ? 'twilight' : 'xcode'}
                            name="model"
                            width="1000px"
                            height="300px"
                            fontSize={14}
                            showPrintMargin={false}
                            showGutter={true}
                            highlightActiveLine={true}
                            value={model}
                            onChange={setModel}
                            setOptions={{
                                enableBasicAutocompletion: true,
                                enableLiveAutocompletion: true,
                                enableSnippets: true,
                                showLineNumbers: true,
                                tabSize: 2,
                                useWorker: false
                            }}
                        />
                    </Box>
                </Box>
                <Box flex={1}>
                    <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                        {translate('templates.preview.title')}
                    </Box>
                    {previewError ? (
                        <Box color="error.main" fontSize="0.875rem">
                            {previewError}
                        </Box>
                    ) : (
                        <Box sx={{ 
                            mt: '2px',
                            width: '700px',
                            height: '820px',
                            border: '1px solid #ddd',
                            borderRadius: '4px',
                            backgroundColor: '#f5f5f5',
                            overflow: 'hidden',
                            display: 'flex',
                            alignItems: 'flex-start',
                            justifyContent: 'center',
                            padding: '16px'
                        }}>
                            <Box sx={{
                                transform: 'scale(0.7)',
                                transformOrigin: 'top center',
                                backgroundColor: 'white',
                                boxShadow: '0 0 10px rgba(0,0,0,0.1)',
                                maxHeight: '100%'
                            }}>
                                <iframe
                                    srcDoc={preview}
                                    title="Template Preview"
                                    style={{
                                        width: '210mm',
                                        height: '297mm',
                                        border: '1px solid #ddd',
                                        backgroundColor: 'white',
                                        display: 'block'
                                    }}
                                    sandbox="allow-same-origin"
                                />
                            </Box>
                        </Box>
                    )}
                </Box>
            </Box>
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
