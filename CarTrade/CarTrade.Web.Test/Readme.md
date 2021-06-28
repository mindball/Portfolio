# Important note: 
```
Fixtures can be shared across assemblies, but collection definitions must be in the 
same assembly as the test that uses them.
```

## Get View Name where ViewResult.ViewName is empty string for Unit Testing
```
If your controller-method is not called through the MVC pipeline, additional 
information are not added to the Controller.ViewData dictionary (which I assumed 
would somehow provide an "action"-key, but couldn't confirm). But since you using 
your controller "outside" the context of the routing-framework etc. there is no 
way it knows about the called method.

If the name of the view was not specified, you 
cannot determine it from the ViewResult returned by the action. At least not 
in the way your controller is being tested (which is totally fine by the way).
```

## Тестване на атрибути на контролер
```
Трудно, минава се през целият процес, тоест тест на цялата система (минава се през атрибута
contex-a, action, model-state), затова е препоръчително да се направи
Integration test
```

## Tip
```
If some behavior can be tested using either a unit test or an integration test, prefer the unit test, 
since it will be almost always be faster. You might have dozens or hundreds of unit tests with many 
different inputs, but just a handful of integration tests covering the most important scenarios.
```