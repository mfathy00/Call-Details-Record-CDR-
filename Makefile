.PHONY: help build run test clean docker-build docker-run docker-stop docker-logs migrate

help: ## Show this help message
	@echo 'Usage: make [target]'
	@echo ''
	@echo 'Targets:'
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "  %-15s %s\n", $$1, $$2}' $(MAKEFILE_LIST)

build: ## Build the application
	dotnet build cdr-api.sln

run: ## Run the application locally
	cd src/Cdr.Api && dotnet run

test: ## Run tests
	dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura

clean: ## Clean build artifacts
	dotnet clean cdr-api.sln
	rm -rf **/bin/ **/obj/

docker-build: ## Build Docker image
	docker build -t cdr-api .

docker-run: ## Run with Docker Compose
	docker-compose up --build -d

docker-stop: ## Stop Docker Compose services
	docker-compose down

docker-logs: ## Show Docker logs
	docker-compose logs -f

migrate: ## Apply database migrations
	dotnet ef database update -p src/Cdr.Infrastructure -s src/Cdr.Api

migrate-add: ## Add new migration (usage: make migrate-add name=migration_name)
	dotnet ef migrations add $(name) -p src/Cdr.Infrastructure -s src/Cdr.Api

dev-setup: ## Setup development environment
	dotnet tool restore
	dotnet restore

# Mac-specific targets
mac-setup: ## Setup development environment on Mac
	brew install postgresql@16
	brew services start postgresql@16
	createdb cdr
	$(MAKE) dev-setup

# Docker-specific targets
docker-setup: ## Setup Docker environment
	docker-compose up -d db
	@echo "Waiting for database to be ready..."
	@until docker-compose exec -T db pg_isready -U postgres -d cdr; do sleep 2; done
	$(MAKE) migrate
