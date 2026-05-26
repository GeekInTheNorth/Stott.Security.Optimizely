function required(name: string): string {
  const value = process.env[name];
  if (!value) {
    throw new Error(`Missing required environment variable: ${name}. See ui-tests/.env.example.`);
  }
  return value;
}

export const env = {
  cmsUsername: required('CMS_USERNAME'),
  cmsPassword: required('CMS_PASSWORD'),
  appOneFrontendUrl: required('APP_ONE_FRONTEND_URL'),
  appOneCmsUrl: required('APP_ONE_CMS_URL'),
  appTwoUrl: required('APP_TWO_URL'),
};
