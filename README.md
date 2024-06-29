### About SNN implementation

Shared Nearest Neighbor (SNN) is a solution to clustering high-dimensional data with the ability to find clusters of varying density. 
SNN assigns objects to a cluster, which share a large number of their nearest neighbors.

Implementation use Gower's distance as similarity metric between samples and triangle inequality method as solution for more effective neighbor finding.

# How to run program

To run program type:
```
	dotnet run
```

To run tests type:
```
	dotnet test
```