# Deployment Phase Documentation

## 1. Infrastructure Architecture

### 1.1 Cloud Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         AWS Cloud Architecture                  │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │                    Internet Gateway                         │ │
│  └─────────────────────────┬───────────────────────────────────┘ │
│                            │                                     │
│  ┌─────────────────────────▼───────────────────────────────────┐ │
│  │                   CloudFront CDN                            │ │
│  │              (Static Assets & API Cache)                   │ │
│  └─────────────────────────┬───────────────────────────────────┘ │
│                            │                                     │
│  ┌─────────────────────────▼───────────────────────────────────┐ │
│  │                 Application Load Balancer                   │ │
│  │                  (SSL Termination)                          │ │
│  └─────────────────────────┬───────────────────────────────────┘ │
│                            │                                     │
│  ┌─────────────────────────▼───────────────────────────────────┐ │
│  │                   Public Subnet                             │ │
│  │  ┌─────────────────┐    ┌─────────────────┐                │ │
│  │  │   ECS Fargate   │    │   ECS Fargate   │                │ │
│  │  │  (Frontend)     │    │  (Backend API)  │                │ │
│  │  └─────────────────┘    └─────────────────┘                │ │
│  └─────────────────────────┬───────────────────────────────────┘ │
│                            │                                     │
│  ┌─────────────────────────▼───────────────────────────────────┐ │
│  │                   Private Subnet                            │ │
│  │  ┌─────────────────┐    ┌─────────────────┐                │ │
│  │  │   RDS Postgres  │    │   ElastiCache   │                │ │
│  │  │   (Database)    │    │     (Redis)     │                │ │
│  │  └─────────────────┘    └─────────────────┘                │ │
│  └─────────────────────────┬───────────────────────────────────┘ │
│                            │                                     │
│  ┌─────────────────────────▼───────────────────────────────────┐ │
│  │                    External Services                        │ │
│  │  ┌─────────────────┐    ┌─────────────────┐                │ │
│  │  │       S3        │    │      SES        │                │ │
│  │  │  (File Storage) │    │  (Email Service)│                │ │
│  │  └─────────────────┘    └─────────────────┘                │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### 1.2 Environment Configuration

#### Production Environment
```yaml
# terraform/environments/production/main.tf
module "vpc" {
  source = "../../modules/vpc"
  
  name = "braavo-prod"
  cidr = "10.0.0.0/16"
  
  azs             = ["us-east-1a", "us-east-1b", "us-east-1c"]
  private_subnets = ["10.0.1.0/24", "10.0.2.0/24", "10.0.3.0/24"]
  public_subnets  = ["10.0.101.0/24", "10.0.102.0/24", "10.0.103.0/24"]
  
  enable_nat_gateway = true
  enable_vpn_gateway = true
  
  tags = {
    Environment = "production"
    Project     = "braavo"
  }
}

module "ecs" {
  source = "../../modules/ecs"
  
  cluster_name = "braavo-prod"
  vpc_id       = module.vpc.vpc_id
  
  services = {
    frontend = {
      image           = "braavo/frontend:latest"
      cpu             = 1024
      memory          = 2048
      desired_count   = 3
      port            = 3000
      health_check_path = "/health"
    }
    backend = {
      image           = "braavo/backend:latest"
      cpu             = 2048
      memory          = 4096
      desired_count   = 3
      port            = 3001
      health_check_path = "/api/health"
    }
  }
  
  tags = {
    Environment = "production"
    Project     = "braavo"
  }
}

module "rds" {
  source = "../../modules/rds"
  
  identifier = "braavo-prod"
  engine     = "postgres"
  engine_version = "15.3"
  
  instance_class = "db.r6g.xlarge"
  allocated_storage = 100
  max_allocated_storage = 1000
  
  db_name  = "braavo"
  username = "braavo"
  
  vpc_security_group_ids = [module.vpc.database_security_group_id]
  db_subnet_group_name   = module.vpc.database_subnet_group
  
  backup_retention_period = 7
  backup_window          = "03:00-04:00"
  maintenance_window     = "sun:04:00-sun:05:00"
  
  tags = {
    Environment = "production"
    Project     = "braavo"
  }
}
```

#### Staging Environment
```yaml
# terraform/environments/staging/main.tf
module "vpc" {
  source = "../../modules/vpc"
  
  name = "braavo-staging"
  cidr = "10.1.0.0/16"
  
  azs             = ["us-east-1a", "us-east-1b"]
  private_subnets = ["10.1.1.0/24", "10.1.2.0/24"]
  public_subnets  = ["10.1.101.0/24", "10.1.102.0/24"]
  
  enable_nat_gateway = true
  
  tags = {
    Environment = "staging"
    Project     = "braavo"
  }
}

module "ecs" {
  source = "../../modules/ecs"
  
  cluster_name = "braavo-staging"
  vpc_id       = module.vpc.vpc_id
  
  services = {
    frontend = {
      image           = "braavo/frontend:staging"
      cpu             = 512
      memory          = 1024
      desired_count   = 2
      port            = 3000
      health_check_path = "/health"
    }
    backend = {
      image           = "braavo/backend:staging"
      cpu             = 1024
      memory          = 2048
      desired_count   = 2
      port            = 3001
      health_check_path = "/api/health"
    }
  }
  
  tags = {
    Environment = "staging"
    Project     = "braavo"
  }
}

module "rds" {
  source = "../../modules/rds"
  
  identifier = "braavo-staging"
  engine     = "postgres"
  engine_version = "15.3"
  
  instance_class = "db.t3.medium"
  allocated_storage = 20
  
  db_name  = "braavo"
  username = "braavo"
  
  vpc_security_group_ids = [module.vpc.database_security_group_id]
  db_subnet_group_name   = module.vpc.database_subnet_group
  
  backup_retention_period = 1
  
  tags = {
    Environment = "staging"
    Project     = "braavo"
  }
}
```

## 2. Containerization

### 2.1 Docker Configuration

#### Frontend Dockerfile
```dockerfile
# packages/frontend/Dockerfile
FROM node:18-alpine AS builder

WORKDIR /app

# Copy package files
COPY package*.json ./
COPY packages/frontend/package*.json ./packages/frontend/
COPY packages/shared/package*.json ./packages/shared/

# Install dependencies
RUN npm ci --only=production

# Copy source code
COPY packages/frontend ./packages/frontend/
COPY packages/shared ./packages/shared/

# Build application
WORKDIR /app/packages/frontend
RUN npm run build

# Production image
FROM nginx:alpine

# Copy built application
COPY --from=builder /app/packages/frontend/dist /usr/share/nginx/html

# Copy nginx configuration
COPY packages/frontend/nginx.conf /etc/nginx/nginx.conf

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
```

#### Backend Dockerfile
```dockerfile
# packages/backend/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /app

# Copy project files
COPY ["src/Braavo.Api/Braavo.Api.csproj", "src/Braavo.Api/"]
COPY ["src/Braavo.Shared/Braavo.Shared.csproj", "src/Braavo.Shared/"]

# Restore dependencies
RUN dotnet restore "src/Braavo.Api/Braavo.Api.csproj"

# Copy source code
COPY src/Braavo.Api/ src/Braavo.Api/
COPY src/Braavo.Shared/ src/Braavo.Shared/

# Build application
WORKDIR /app/src/Braavo.Api
RUN dotnet publish "Braavo.Api.csproj" -c Release -o /app/src/Braavo.Api/publish

# Production image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy built application
COPY --from=builder /app/src/Braavo.Api/publish ./

# Create non-root user
RUN addgroup --gid 1001 --system dotnet
RUN adduser --uid 1001 --system --gid 1001 dotnet

# Change ownership
USER dotnet

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:5000/api/health || exit 1

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "Braavo.Api.dll"]
```

### 2.2 Docker Compose for Development
```yaml
# docker-compose.dev.yml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: braavo_dev
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 30s
      timeout: 10s
      retries: 5

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 30s
      timeout: 10s
      retries: 5

  backend:
    build:
      context: .
      dockerfile: src/Braavo.Api/Dockerfile.dev
    ports:
      - "5000:5000"
      - "5001:5001"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5000;https://+:5001
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=braavo_dev;Username=postgres;Password=password
      - ConnectionStrings__Redis=redis:6379
      - JWT__Secret=dev-secret
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
    volumes:
      - ./src/Braavo.Api:/app/src/Braavo.Api
      - ./src/Braavo.Shared:/app/src/Braavo.Shared
    command: dotnet watch run

  frontend:
    build:
      context: .
      dockerfile: packages/frontend/Dockerfile.dev
    ports:
      - "3000:3000"
    environment:
      - REACT_APP_API_URL=http://localhost:3001/api
    depends_on:
      - backend
    volumes:
      - ./packages/frontend:/app/packages/frontend
      - ./packages/shared:/app/packages/shared
      - /app/packages/frontend/node_modules
    command: npm run dev

volumes:
  postgres_data:
  redis_data:
```

## 3. CI/CD Pipeline

### 3.1 GitHub Actions Workflow
```yaml
# .github/workflows/deploy.yml
name: Deploy to AWS

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  AWS_REGION: us-east-1
  ECR_REPOSITORY_FRONTEND: braavo/frontend
  ECR_REPOSITORY_BACKEND: braavo/backend

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: password
          POSTGRES_DB: braavo_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432
      
      redis:
        image: redis:7-alpine
        ports:
          - 6379:6379

    steps:
      - name: Checkout
        uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'npm'

      - name: Install frontend dependencies
        run: npm ci
        working-directory: ./src/Frontend

      - name: Run frontend linting
        run: npm run lint
        working-directory: ./src/Frontend

      - name: Run frontend type checking
        run: npm run type-check
        working-directory: ./src/Frontend

      - name: Restore .NET dependencies
        run: dotnet restore
        working-directory: ./src/Braavo.Api

      - name: Run backend tests
        run: dotnet test --logger trx --collect:"XPlat Code Coverage"
        working-directory: ./src/Braavo.Api
        env:
          ConnectionStrings__DefaultConnection: Host=localhost;Database=braavo_test;Username=postgres;Password=password
          ConnectionStrings__Redis: localhost:6379

      - name: Run frontend tests
        run: npm run test
        working-directory: ./src/Frontend

      - name: Build backend
        run: dotnet publish -c Release -o ./publish
        working-directory: ./src/Braavo.Api

      - name: Build frontend
        run: npm run build
        working-directory: ./src/Frontend

  build-and-deploy:
    needs: test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main' || github.ref == 'refs/heads/develop'
    
    steps:
      - name: Checkout
        uses: actions/checkout@v3

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v2
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ env.AWS_REGION }}

      - name: Login to Amazon ECR
        id: login-ecr
        uses: aws-actions/amazon-ecr-login@v1

      - name: Build, tag, and push frontend image
        id: build-frontend
        env:
          ECR_REGISTRY: ${{ steps.login-ecr.outputs.registry }}
          IMAGE_TAG: ${{ github.sha }}
        run: |
          docker build -t $ECR_REGISTRY/$ECR_REPOSITORY_FRONTEND:$IMAGE_TAG -f packages/frontend/Dockerfile .
          docker push $ECR_REGISTRY/$ECR_REPOSITORY_FRONTEND:$IMAGE_TAG
          echo "image=$ECR_REGISTRY/$ECR_REPOSITORY_FRONTEND:$IMAGE_TAG" >> $GITHUB_OUTPUT

      - name: Build, tag, and push backend image
        id: build-backend
        env:
          ECR_REGISTRY: ${{ steps.login-ecr.outputs.registry }}
          IMAGE_TAG: ${{ github.sha }}
        run: |
          docker build -t $ECR_REGISTRY/$ECR_REPOSITORY_BACKEND:$IMAGE_TAG -f packages/backend/Dockerfile .
          docker push $ECR_REGISTRY/$ECR_REPOSITORY_BACKEND:$IMAGE_TAG
          echo "image=$ECR_REGISTRY/$ECR_REPOSITORY_BACKEND:$IMAGE_TAG" >> $GITHUB_OUTPUT

      - name: Deploy to staging
        if: github.ref == 'refs/heads/develop'
        run: |
          aws ecs update-service --cluster braavo-staging --service braavo-frontend-staging --force-new-deployment
          aws ecs update-service --cluster braavo-staging --service braavo-backend-staging --force-new-deployment

      - name: Deploy to production
        if: github.ref == 'refs/heads/main'
        run: |
          aws ecs update-service --cluster braavo-prod --service braavo-frontend-prod --force-new-deployment
          aws ecs update-service --cluster braavo-prod --service braavo-backend-prod --force-new-deployment

      - name: Wait for deployment
        run: |
          if [ "${{ github.ref }}" == "refs/heads/main" ]; then
            aws ecs wait services-stable --cluster braavo-prod --services braavo-frontend-prod braavo-backend-prod
          else
            aws ecs wait services-stable --cluster braavo-staging --services braavo-frontend-staging braavo-backend-staging
          fi

      - name: Run smoke tests
        run: |
          npm run test:smoke
        env:
          ENVIRONMENT: ${{ github.ref == 'refs/heads/main' && 'production' || 'staging' }}
```

### 3.2 Deployment Scripts
```bash
#!/bin/bash
# scripts/deploy.sh

set -e

ENVIRONMENT=$1
AWS_REGION="us-east-1"
ECR_REGISTRY="123456789012.dkr.ecr.us-east-1.amazonaws.com"

if [ -z "$ENVIRONMENT" ]; then
  echo "Usage: $0 [staging|production]"
  exit 1
fi

if [ "$ENVIRONMENT" != "staging" ] && [ "$ENVIRONMENT" != "production" ]; then
  echo "Environment must be 'staging' or 'production'"
  exit 1
fi

echo "Deploying to $ENVIRONMENT environment..."

# Build and push Docker images
echo "Building Docker images..."
docker build -t braavo/frontend:$ENVIRONMENT -f packages/frontend/Dockerfile .
docker build -t braavo/backend:$ENVIRONMENT -f packages/backend/Dockerfile .

# Tag for ECR
docker tag braavo/frontend:$ENVIRONMENT $ECR_REGISTRY/braavo/frontend:$ENVIRONMENT
docker tag braavo/backend:$ENVIRONMENT $ECR_REGISTRY/braavo/backend:$ENVIRONMENT

# Push to ECR
echo "Pushing to ECR..."
aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $ECR_REGISTRY
docker push $ECR_REGISTRY/braavo/frontend:$ENVIRONMENT
docker push $ECR_REGISTRY/braavo/backend:$ENVIRONMENT

# Update ECS services
echo "Updating ECS services..."
aws ecs update-service --cluster braavo-$ENVIRONMENT --service braavo-frontend-$ENVIRONMENT --force-new-deployment
aws ecs update-service --cluster braavo-$ENVIRONMENT --service braavo-backend-$ENVIRONMENT --force-new-deployment

# Wait for deployment to complete
echo "Waiting for deployment to complete..."
aws ecs wait services-stable --cluster braavo-$ENVIRONMENT --services braavo-frontend-$ENVIRONMENT braavo-backend-$ENVIRONMENT

echo "Deployment completed successfully!"

# Run health checks
echo "Running health checks..."
sleep 30
curl -f https://api.$ENVIRONMENT.braavo.com/health || exit 1
curl -f https://$ENVIRONMENT.braavo.com/health || exit 1

echo "Health checks passed!"
```

## 4. Database Migration

### 4.1 Migration Strategy
```typescript
// scripts/migrate.ts
import { PrismaClient } from '@prisma/client';
import { exec } from 'child_process';
import { promisify } from 'util';

const execAsync = promisify(exec);
const prisma = new PrismaClient();

interface MigrationOptions {
  environment: 'staging' | 'production';
  dryRun?: boolean;
  force?: boolean;
}

export class MigrationManager {
  async runMigrations(options: MigrationOptions): Promise<void> {
    console.log(`Running migrations for ${options.environment} environment...`);
    
    if (options.dryRun) {
      console.log('Dry run mode - no actual changes will be made');
      return this.dryRunMigrations();
    }

    // Backup database before migration
    await this.backupDatabase(options.environment);

    try {
      // Run Prisma migrations
      await this.runPrismaMigrations();
      
      // Run custom data migrations
      await this.runCustomMigrations();
      
      console.log('Migrations completed successfully!');
    } catch (error) {
      console.error('Migration failed:', error);
      
      if (options.environment === 'production') {
        console.log('Attempting to restore from backup...');
        await this.restoreFromBackup(options.environment);
      }
      
      throw error;
    }
  }

  private async backupDatabase(environment: string): Promise<void> {
    console.log('Creating database backup...');
    
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    const backupName = `braavo-${environment}-backup-${timestamp}`;
    
    const { stdout, stderr } = await execAsync(
      `pg_dump ${process.env.DATABASE_URL} > backups/${backupName}.sql`
    );
    
    if (stderr) {
      console.error('Backup error:', stderr);
      throw new Error('Database backup failed');
    }
    
    console.log(`Backup created: ${backupName}.sql`);
  }

  private async runPrismaMigrations(): Promise<void> {
    console.log('Running Prisma migrations...');
    
    const { stdout, stderr } = await execAsync('npx prisma migrate deploy');
    
    if (stderr && !stderr.includes('warning')) {
      console.error('Prisma migration error:', stderr);
      throw new Error('Prisma migration failed');
    }
    
    console.log('Prisma migrations completed');
  }

  private async runCustomMigrations(): Promise<void> {
    console.log('Running custom migrations...');
    
    // Example: Data migration for user preferences
    await this.migrateUserPreferences();
    
    console.log('Custom migrations completed');
  }

  private async migrateUserPreferences(): Promise<void> {
    const users = await prisma.user.findMany({
      where: {
        preferences: null
      }
    });

    for (const user of users) {
      await prisma.user.update({
        where: { id: user.id },
        data: {
          preferences: {
            theme: 'light',
            notifications: true,
            language: 'en'
          }
        }
      });
    }

    console.log(`Updated preferences for ${users.length} users`);
  }

  private async dryRunMigrations(): Promise<void> {
    console.log('Performing dry run...');
    
    // Check pending migrations
    const { stdout } = await execAsync('npx prisma migrate status');
    console.log('Migration status:', stdout);
    
    // Validate migration files
    const { stdout: validateOutput } = await execAsync('npx prisma migrate diff --preview-feature');
    console.log('Migration diff:', validateOutput);
  }

  private async restoreFromBackup(environment: string): Promise<void> {
    console.log('Restoring from backup...');
    
    // Implementation would restore from the most recent backup
    // This is a simplified example
    throw new Error('Backup restoration not implemented');
  }
}

// CLI usage
if (require.main === module) {
  const environment = process.argv[2] as 'staging' | 'production';
  const dryRun = process.argv.includes('--dry-run');
  const force = process.argv.includes('--force');

  if (!environment) {
    console.error('Usage: npm run migrate [staging|production] [--dry-run] [--force]');
    process.exit(1);
  }

  const migrationManager = new MigrationManager();
  migrationManager.runMigrations({ environment, dryRun, force })
    .catch(console.error)
    .finally(() => prisma.$disconnect());
}
```

## 5. Environment Variables

### 5.1 Environment Configuration
```bash
# .env.production
NODE_ENV=production
PORT=3001

# Database
DATABASE_URL=postgresql://username:password@prod-db.amazonaws.com:5432/braavo
DATABASE_SSL=true
DATABASE_POOL_SIZE=20

# Redis
REDIS_URL=redis://prod-redis.amazonaws.com:6379
REDIS_CLUSTER_MODE=true

# JWT
JWT_SECRET=super-secret-production-key
JWT_EXPIRATION=24h

# AWS
AWS_REGION=us-east-1
AWS_ACCESS_KEY_ID=AKIAIOSFODNN7EXAMPLE
AWS_SECRET_ACCESS_KEY=wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY
AWS_S3_BUCKET=braavo-prod-assets

# OpenAI
OPENAI_API_KEY=sk-example-key
OPENAI_MODEL=gpt-4
OPENAI_MAX_TOKENS=4000

# External APIs
FIGMA_CLIENT_ID=figma-client-id
FIGMA_CLIENT_SECRET=figma-client-secret
GITHUB_CLIENT_ID=github-client-id
GITHUB_CLIENT_SECRET=github-client-secret

# Monitoring
SENTRY_DSN=https://example@sentry.io/123456
NEW_RELIC_LICENSE_KEY=example-key
LOG_LEVEL=info

# Email
SMTP_HOST=smtp.sendgrid.net
SMTP_PORT=587
SMTP_USER=apikey
SMTP_PASS=sendgrid-api-key
FROM_EMAIL=noreply@braavo.com

# Security
CORS_ORIGIN=https://braavo.com
RATE_LIMIT_WINDOW=15
RATE_LIMIT_MAX=100
```

### 5.2 Secrets Management
```yaml
# kubernetes/secrets.yaml
apiVersion: v1
kind: Secret
metadata:
  name: braavo-secrets
  namespace: braavo
type: Opaque
data:
  database-url: <base64-encoded-value>
  jwt-secret: <base64-encoded-value>
  openai-api-key: <base64-encoded-value>
  aws-access-key-id: <base64-encoded-value>
  aws-secret-access-key: <base64-encoded-value>
  figma-client-secret: <base64-encoded-value>
  github-client-secret: <base64-encoded-value>
  smtp-password: <base64-encoded-value>
```

## 6. Monitoring and Logging

### 6.1 Application Monitoring
```typescript
// src/middleware/monitoring.ts
import { Request, Response, NextFunction } from 'express';
import { Counter, Histogram, register } from 'prom-client';

const httpRequestsTotal = new Counter({
  name: 'http_requests_total',
  help: 'Total number of HTTP requests',
  labelNames: ['method', 'route', 'status_code']
});

const httpRequestDuration = new Histogram({
  name: 'http_request_duration_seconds',
  help: 'Duration of HTTP requests in seconds',
  labelNames: ['method', 'route', 'status_code'],
  buckets: [0.1, 0.5, 1, 2, 5, 10]
});

export const metricsMiddleware = (req: Request, res: Response, next: NextFunction) => {
  const start = Date.now();
  
  res.on('finish', () => {
    const duration = (Date.now() - start) / 1000;
    const route = req.route?.path || req.path;
    
    httpRequestsTotal.labels(req.method, route, res.statusCode.toString()).inc();
    httpRequestDuration.labels(req.method, route, res.statusCode.toString()).observe(duration);
  });
  
  next();
};

export const metricsEndpoint = (req: Request, res: Response) => {
  res.set('Content-Type', register.contentType);
  res.end(register.metrics());
};
```

### 6.2 Logging Configuration
```typescript
// src/utils/logger.ts
import winston from 'winston';
import { CloudWatchLogsClient } from '@aws-sdk/client-cloudwatch-logs';

const logger = winston.createLogger({
  level: process.env.LOG_LEVEL || 'info',
  format: winston.format.combine(
    winston.format.timestamp(),
    winston.format.errors({ stack: true }),
    winston.format.json()
  ),
  defaultMeta: {
    service: 'braavo-api',
    environment: process.env.NODE_ENV,
    version: process.env.APP_VERSION
  },
  transports: [
    new winston.transports.File({ filename: 'logs/error.log', level: 'error' }),
    new winston.transports.File({ filename: 'logs/combined.log' })
  ]
});

if (process.env.NODE_ENV !== 'production') {
  logger.add(new winston.transports.Console({
    format: winston.format.simple()
  }));
}

if (process.env.NODE_ENV === 'production') {
  // Add CloudWatch transport for production
  logger.add(new winston.transports.Console({
    format: winston.format.json()
  }));
}

export { logger };
```

## 7. Health Checks and Monitoring

### 7.1 Health Check Endpoints
```typescript
// src/routes/health.ts
import { Router } from 'express';
import { prisma } from '../database/prisma';
import { redis } from '../database/redis';

const router = Router();

interface HealthCheck {
  status: 'healthy' | 'unhealthy';
  timestamp: string;
  version: string;
  uptime: number;
  services: {
    database: 'healthy' | 'unhealthy';
    redis: 'healthy' | 'unhealthy';
    external_apis: 'healthy' | 'unhealthy';
  };
}

router.get('/health', async (req, res) => {
  const healthCheck: HealthCheck = {
    status: 'healthy',
    timestamp: new Date().toISOString(),
    version: process.env.APP_VERSION || '1.0.0',
    uptime: process.uptime(),
    services: {
      database: 'healthy',
      redis: 'healthy',
      external_apis: 'healthy'
    }
  };

  try {
    // Check database connection
    await prisma.$queryRaw`SELECT 1`;
  } catch (error) {
    healthCheck.services.database = 'unhealthy';
    healthCheck.status = 'unhealthy';
  }

  try {
    // Check Redis connection
    await redis.ping();
  } catch (error) {
    healthCheck.services.redis = 'unhealthy';
    healthCheck.status = 'unhealthy';
  }

  try {
    // Check external APIs
    const openaiResponse = await fetch('https://api.openai.com/v1/models', {
      headers: {
        'Authorization': `Bearer ${process.env.OPENAI_API_KEY}`
      }
    });
    
    if (!openaiResponse.ok) {
      throw new Error('OpenAI API not responding');
    }
  } catch (error) {
    healthCheck.services.external_apis = 'unhealthy';
    healthCheck.status = 'unhealthy';
  }

  const statusCode = healthCheck.status === 'healthy' ? 200 : 503;
  res.status(statusCode).json(healthCheck);
});

router.get('/ready', async (req, res) => {
  // Readiness check - more thorough than health check
  try {
    await prisma.$queryRaw`SELECT 1`;
    await redis.ping();
    
    res.status(200).json({ 
      status: 'ready',
      timestamp: new Date().toISOString()
    });
  } catch (error) {
    res.status(503).json({ 
      status: 'not ready',
      timestamp: new Date().toISOString(),
      error: error.message
    });
  }
});

export { router as healthRouter };
```

### 7.2 Alerting Configuration
```yaml
# monitoring/alerts.yml
groups:
  - name: braavo-alerts
    rules:
      - alert: HighErrorRate
        expr: rate(http_requests_total{status_code=~"5.."}[5m]) > 0.1
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "High error rate detected"
          description: "Error rate is above 10% for 5 minutes"

      - alert: HighResponseTime
        expr: histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m])) > 2
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High response time detected"
          description: "95th percentile response time is above 2 seconds"

      - alert: DatabaseDown
        expr: up{job="postgres"} == 0
        for: 1m
        labels:
          severity: critical
        annotations:
          summary: "Database is down"
          description: "PostgreSQL database is not responding"

      - alert: RedisDown
        expr: up{job="redis"} == 0
        for: 1m
        labels:
          severity: warning
        annotations:
          summary: "Redis is down"
          description: "Redis cache is not responding"

      - alert: HighMemoryUsage
        expr: container_memory_usage_bytes / container_spec_memory_limit_bytes > 0.8
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High memory usage"
          description: "Memory usage is above 80%"

      - alert: LowDiskSpace
        expr: node_filesystem_avail_bytes / node_filesystem_size_bytes < 0.1
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "Low disk space"
          description: "Disk space is below 10%"
```

This comprehensive deployment documentation provides everything needed to deploy and maintain the ChatPRD Clone platform in production environments, including infrastructure setup, containerization, CI/CD pipelines, monitoring, and alerting. 