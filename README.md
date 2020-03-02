# Resource Booking

Resource management service. 

Features:
1. You can add resources via data/resources.json
2. You can book resource for some time
3. You can release resource
4. You can extend reservation time for resource
5. You can get in line for resource
6. You can set up notifications (now only in mattermost)
7. You can edit resources

## Getting started

1. Copy `docker-compose.yml` to target folder.
2. Create folder `data` inside target folder.
3. Put into `data` file `resources.json` with initial resources. Example:
```
{
  "groups" : [
    {
      "name": "resource-group 1",
      "resources": [
        {
          "name": "resource-1"
        },
        
        {
          "name": "resource-1"
        }
      ]
    },

    {
      "name": "resource-group 2",
      "resources": [
        {
          "name": "resource-1"
        },
        
        {
          "name": "resource-1"
        }
      ]
    }
  ]
}

```
4. Create folder for certificates `https` inside target
5. Inside `https`:
  a. Bash: `openssl req -new -x509 -newkey rsa:2048 -keyout <hostname>.key -out <hostname>.cer -days 365 -subj /CN=<hostname>`
  b. Bash: `penssl pkcs12 -export -out certificate.pfx -inkey <hostname>.key -in <hostname>.cer`
6. Set actual `gitlab` parameters in `docker-compose.yml`
7. Set actual `notifications` parameters in `docker-compose.yml` (or remove mattermost section)
8. Bash: `docker-compose up -d`

### Additional parameters

You can add additional parameters to environment variables in docker-compose.yml:
1. `booking__MaxBookingPeriodInMinutes` default 1440
2. `booking__MinBookingPeriodInMinutes` default 20
3. `notifications__hostname` is used for resource display name in notifications, default "localhost"
4. `notifications__notifyBeforeEndingOfReservationInMinutes` default 10
5. `notifications__mattermost__hook` is used for notifications to mattermost


## Demo 
[resources-booking.herokuapp.com](http://resources-booking.herokuapp.com)
