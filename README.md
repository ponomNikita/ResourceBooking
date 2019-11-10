# ResourceBooking

Resource management service.

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
4. Set actual `gitlab` parameters in `docker-compose.yml`
4. Bash: `docker-compose up -d`


