export const environment = {
    production: false,
    apiendpoint: 'https://localhost:44393/',
    featureFlag: {
        FeatureMaterial: true,
    },
    authConfig: {
        auth: {
            authority: 'https://login.microsoftonline.com/1c4e72e7-52a1-475f-b469-0d4fbf3eae2e',
            clientId: 'c74e839f-fbfc-4d9a-a06d-00602c3e3e40'
        }
    },
    authScopes: ['api://c74e839f-fbfc-4d9a-a06d-00602c3e3e40/access']
};
