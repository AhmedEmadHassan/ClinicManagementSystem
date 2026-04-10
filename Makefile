include .env

ifeq ($(APP_ENV), prod)
	COMPOSE_FILE = -f docker-compose.yml -f docker-compose.prod.yml
else
	COMPOSE_FILE = -f docker-compose.yml -f docker-compose.dev.yml
endif

up:
	docker compose $(COMPOSE_FILE) up --build -d

down:
	docker compose $(COMPOSE_FILE) down

logs:
	docker compose $(COMPOSE_FILE) logs -f api

ps:
	docker compose $(COMPOSE_FILE) ps

clean:
	docker compose $(COMPOSE_FILE) down -v

#make up        # Start
#make down      # Stop
#make logs      # View logs
#make clean     # Stop and remove volumes