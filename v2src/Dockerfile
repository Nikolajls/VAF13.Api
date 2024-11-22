FROM golang:1.23.3-alpine3.20 AS builder
COPY . .
RUN go mod download
RUN CGO_ENABLED=0 GOOS=linux go build -a -installsuffix cgo -o /app/build

FROM alpine
RUN apk add --no-cache ca-certificates && update-ca-certificates
COPY --from=builder /app/build /app
EXPOSE 8080 8080
ENTRYPOINT ["/app"]