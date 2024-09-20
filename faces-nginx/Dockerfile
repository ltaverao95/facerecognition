FROM nginx:latest

USER root
RUN apt update
RUN apt upgrade -y
RUN apt install nano apt-utils certbot python3-certbot-nginx -y

# Copy your custom addons to the addons directory
COPY ./config /etc/nginx/conf.d