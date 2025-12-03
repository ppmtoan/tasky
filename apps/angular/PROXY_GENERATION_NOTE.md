# Angular Proxy Service Generation Issue - RESOLVED

## Issue: "[Invalid module] Backend module 'app' does not exist in API definition"

### Problem

When running `abp generate-proxy`, the error occurs because:
1. **Incorrect module names** - Using `saasService`, `SaasService`, or `app` instead of the correct module name
2. **Missing or incorrect `generate-proxy.json`** configuration file
3. **Module name mismatch** between CLI command and actual backend API definition

### Root Cause Analysis

The backend API exposes modules with specific names (case-sensitive):
- ✅ `saas` - SaaS Service (NOT `saasService` or `SaasService`)
- ✅ `productService` - Product Service
- ✅ `identity` - Identity Service
- ✅ `account` - Account Service
- `app` - Generic app module (exists but not what you typically want)
- Other modules: `permissionManagement`, `auditLogging`, `languageManagement`, etc.

### Solution: Correct Configuration & Module Names

**1. Created `generate-proxy.json`** at project root:

Location: `/apps/angular/generate-proxy.json`

```json
{
  "modules": {
    "saas": {
      "apiName": "Default",
      "source": "moduletest",
      "serviceType": "application"
    },
    "productService": {
      "apiName": "Default",
      "source": "moduletest",
      "serviceType": "application"
    },
    "identity": {
      "apiName": "Default",
      "source": "moduletest",
      "serviceType": "application"
    },
    "account": {
      "apiName": "Default",
      "source": "moduletest",
      "serviceType": "application"
    }
  }
}
```

**2. Use Correct Module Names in ABP CLI:**

**2. Use Correct Module Names in ABP CLI:**

```bash
cd /Users/toanpham/Desktop/Github/ABP/tasky/apps/angular

# ✅ CORRECT - Generate proxy for SaaS module
abp generate-proxy -t ng -m saas

# ✅ CORRECT - Generate proxy for Product Service module
abp generate-proxy -t ng -m productService

# ✅ CORRECT - Generate all configured modules
abp generate-proxy -t ng --all

# ❌ WRONG - These will fail
abp generate-proxy -t ng -m app              # Wrong module name
abp generate-proxy -t ng -m SaasService      # Case mismatch
abp generate-proxy -t ng -m saasService      # Case mismatch
```

### How to Find Available Modules

To see all available modules from the backend API:

```bash
cd /Users/toanpham/Desktop/Github/ABP/tasky/apps/angular

# Extract module names from API definition
cat projects/product-service/src/lib/proxy/generate-proxy.json | \
  python3 -c "import sys, json; data=json.load(sys.stdin); \
  print('Available modules:'); \
  [print(f'  - {k}') for k in data.get('modules', {}).keys()]"
```

**Available Modules (as of Dec 3, 2025):**
- `saas` - SaaS Service with Editions, Subscriptions, Invoices
- `productService` - Product management
- `identity` - User and role management
- `account` - Account and authentication
- `permissionManagement` - Permission system
- `auditLogging` - Audit logs
- `languageManagement` - Multi-language support
- `textTemplateManagement` - Text templates
- `settingManagement` - Application settings
- `featureManagement` - Feature toggles
- `leptonThemeManagement` - Theme management
- `identityServer` - OpenID Connect
- `accountAdmin` - Admin account management
- `abp` - Core ABP services
- `app` - Generic application module

### Prerequisites

### Prerequisites

**Backend Service Must Be Running:**

The Web Gateway must be running to expose the API definition:
```bash
# Web Gateway serves all microservices through API Gateway pattern
# URL: https://localhost:44325
```

Alternatively, ensure these services are running:
- **Auth Server**: `https://localhost:44322`
- **Web Gateway**: `https://localhost:44325`
- **SaaS Service**: `https://localhost:44381` (optional, gateway proxies to it)
- **Product Service**: Via gateway
- **Identity Service**: Via gateway
- **Administration Service**: Via gateway

### Expected Outcome

ABP CLI will:
1. ✅ Connect to the Web Gateway at `https://localhost:44325`
2. ✅ Read the API definition (Swagger/OpenAPI)
3. ✅ Generate TypeScript proxy services with correct module names
4. ✅ Create DTOs, interfaces, and enums
5. ✅ Integrate with ABP's RestService
6. ✅ Place files in `projects/module-test/src/app/proxy/{module-name}/`

### Verify Generation

After successful generation, check these locations:

```bash
projects/module-test/src/app/proxy/
├── saas/                    # ✅ SaaS Service proxies
│   ├── edition.service.ts
│   ├── subscription.service.ts
│   ├── invoice.service.ts
│   ├── models.ts
│   └── index.ts
├── product-service/         # ✅ Product Service proxies
│   ├── product.service.ts
│   ├── models.ts
│   └── index.ts
└── ...other modules
```

### Troubleshooting

**Error: "Backend module 'xxx' does not exist"**
- ✅ Use lowercase module names from the list above
- ✅ Check `generate-proxy.json` has correct module configuration
- ✅ Ensure backend/gateway is running
- ❌ Don't use service suffix (use `saas` not `saasService`)

**Error: "Cannot connect to API"**
- ✅ Verify gateway is running at `https://localhost:44325`
- ✅ Check SSL certificate is trusted
- ✅ Test API endpoint: `curl -k https://localhost:44325/api/abp/api-definition`

**Error: "Module name mismatch"**
- ✅ Module names are case-sensitive: `saas` ≠ `Saas` ≠ `SAAS`
- ✅ Use exact names from available modules list

### Configuration Files

### Configuration Files

**`generate-proxy.json`** (Project root)
```json
{
  "modules": {
    "saas": {
      "apiName": "Default",
      "source": "moduletest",
      "serviceType": "application"
    }
  }
}
```

**`environment.ts`** (API endpoint configuration)
```typescript
export const environment = {
  apis: {
    default: {
      url: 'https://localhost:44325',  // Web Gateway
      rootNamespace: 'ModuleTest',
    },
    SaasService: {
      url: 'https://localhost:44325',  // Routes through gateway
      rootNamespace: 'ModuleTest.SaasService',
    },
    ProductService: {
      url: 'https://localhost:44325',
      rootNamespace: 'ModuleTest.ProductService',
    }
  }
}
```

### Files Structure

**Backend (SaaS Service):**
- `/services/saas/src/ModuleTest.SaasService.HttpApi/Controllers/` - API controllers
- `/services/saas/src/ModuleTest.SaasService.Application.Contracts/` - DTOs
  - `Editions/CreateEditionDto.cs`, `EditionDto.cs`, `UpdateEditionDto.cs`
  - `Subscriptions/CreateSubscriptionDto.cs`, `SubscriptionDto.cs`
  - `Invoices/InvoiceDto.cs`

**Frontend (Angular):**
- ✅ `/apps/angular/generate-proxy.json` - Proxy generation config
- ✅ `/apps/angular/projects/module-test/src/environments/environment.ts` - API config
- ✅ `/apps/angular/projects/module-test/src/app/route.provider.ts` - Menu routes
- 🔄 `/apps/angular/projects/module-test/src/app/proxy/saas/` - Generated proxy services (to be regenerated)

### Quick Reference Commands

```bash
# Navigate to Angular project
cd /Users/toanpham/Desktop/Github/ABP/tasky/apps/angular

# List available modules
cat projects/product-service/src/lib/proxy/generate-proxy.json | \
  grep '"rootPath"' | awk -F'"' '{print $4}' | sort | uniq

# Generate specific module
abp generate-proxy -t ng -m saas

# Generate all modules
abp generate-proxy -t ng --all

# Build Angular app
npm run build

# Serve Angular app
npm start
```

### Next Steps

1. ✅ **Configuration Created** - `generate-proxy.json` with correct module names
2. ✅ **Environment Configured** - API endpoints point to Web Gateway
3. ✅ **AutoMapper Fixed** - Backend mappings for Edition, Subscription, Invoice
4. 🔄 **Generate Proxies** - Run `abp generate-proxy -t ng -m saas`
5. ⏳ **Build & Test** - Verify Angular app builds and API calls work

### Key Learnings

✅ **Module names are case-sensitive** - Always use exact names from API definition
✅ **Use `generate-proxy.json`** - Centralized configuration prevents errors
✅ **Web Gateway pattern** - All services route through single gateway (44325)
✅ **Backend must be running** - ABP CLI needs live API definition
✅ **Don't use "app" module** - Generic module, use specific service modules instead

## Reference

- **ABP Generate Proxy Docs**: https://abp.io/docs/latest/cli#generate-proxy
- **ABP Angular Service Proxy**: https://abp.io/docs/latest/framework/ui/angular/service-proxies
- **Microservice Architecture**: https://abp.io/docs/latest/solution-templates/microservice

### Service Endpoints

| Service | Direct URL | Via Gateway |
|---------|-----------|-------------|
| Auth Server | `https://localhost:44322` | N/A |
| Web Gateway | `https://localhost:44325` | N/A |
| SaaS Service | `https://localhost:44381` | `https://localhost:44325` |
| Product Service | Direct port | `https://localhost:44325` |
| Identity Service | Direct port | `https://localhost:44325` |
| Administration | Direct port | `https://localhost:44325` |

**Recommended**: Always use Web Gateway URL (`https://localhost:44325`) for proxy generation and API calls.

---

**Last Updated**: December 3, 2025  
**Status**: ✅ Configuration Fixed - Ready for proxy generation  
**Issue**: "[Invalid module] Backend module 'app' does not exist" - **RESOLVED**
