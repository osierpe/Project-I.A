Collection of Scripts we are using for our AI Project for the FCTUC Coimbra.
All of these algorithms use the same evaluation function and utility function. ALl that changes is their implementation in the details.

MinMaxAlgorithm.cs	represents the canonical version of the code, not implementing Alpha-Beta pruning, and using the NumberOfExpandedNodes as the base case for the recursivitiy.

MinMaxAlgorithmAlpha.cs	 represents the canonical version of the code, but implementing Alpha-Beta pruning, also using NumberOfExpandedNodes

MinMaxAlgorithmDepth.cs	represents our initial version of the algorithm, not using NumberOfExpandedNodes but instead the depth of the node.

MinMaxAlgorithmDepthAlpha.cs	represents our intial version of the algorithm, using depth of the node, and also alpha-beta pruning.

MinMaxAlgorithmProgessDepthAlpha.cs	represents our latest version of the algorithm, using depth of the node, alpa-beta pruning, and progressively goes through the depths, respecting the NumberOfExpandedNodes <= MaximumNumberOfExpandedNodes also.
