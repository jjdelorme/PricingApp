
## Using .NET 6 Preview with Cloud Native buildpack:
```bash
pack build pricingapp --builder gcr.io/buildpacks/builder:v1 --env  GOOGLE_RUNTIME_VERSION=6.0.100-preview.6.21355.2
```

You can substitute `GOOGLE_RUNTIME_VERSION=6.0.100-preview.7.21379.14` for the a different version for example.

## Or use Cloud Build
```bash
gcloud builds submit --pack=env=GOOGLE_RUNTIME_VERSION=6.0.100-preview.7.21379.14,image=gcr.io/$PROJECT_ID/pricingapp
```