# global
- "Async" suffixes only when there is a sync counterpart

# controllers
- "Controller" suffix required
- method names the same as corresponding http method
- can perform only basic parameter validations
- can transform data types to / from services

# entities
- "Entity" suffix required (by convention)
- have sanitize and validate methods
- use "CollectionManager" for collection properties

# models
- "Model" suffix not required
- config type definitions have "Config" suffixes
- event type definitions have "Event" suffixes
- event names can be noun only (use creationEvent, not createdEvent)

# services
- "Service" suffix not required
- configurations passed by IOptions and Config models
