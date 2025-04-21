# Sternban
This project is a back-end hobby project created for me to learn about clean arcitechture and DDD and CI/CD using docker and github actions (this is the reason this project is public).Got it — here’s a simpler and more straightforward version of the Installation & Deployment (Docker) section, focusing on clarity without over-explaining:

## Installation & Deployment (Docker)

This project uses Docker and Docker Compose to set up the environment for development and deployment.

### Dockerfile

The Dockerfile builds the .NET application using a multi-stage setup and exposes the ports:
`8080`, `8081`

### Docker Compose

`docker-compose.yml` defines three services that this project integrate with:

- **server**: Builds the backend app, runs on port `8085`, and connects to MongoDB.
- **mongo**: MongoDB service with persistent volume.
- **frontend**: Optional frontend served from a prebuilt image, runs on port `4200`.

## Start the project 
The easiest way to start everything run the following command from the project folder:

```bash
docker-compose up --build
```

Now you reach the project on local host:
- The backend [api definitions](http://localhost:8085/scalar/v1) 
- The frontend [frontpage](http://localhost:4200/)