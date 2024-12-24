FROM node:18 AS build
WORKDIR /app

# Copy package.json and install dependencies
COPY package*.json ./
RUN npm install

# Copy all project files
COPY . .

# Set OpenSSL legacy provider for compatibility
ENV NODE_OPTIONS=--openssl-legacy-provider

# Build the application
RUN npm run build

# Serve the built application with Nginx
FROM nginx:1.23-alpine
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]