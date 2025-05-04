using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoSum : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int[] nums = new int[] {2, 7, 11, 15};
        TwoSums(nums, 9);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int[] TwoSums(int[] nums, int target) {
            Dictionary<int, int> map = new Dictionary<int, int>();
            int complement = 0;
            for(int i= 0; i<nums.Length; i++)
            {

                // target = complement + nums[i];

                complement = target - nums[i];
                if(map.ContainsKey(complement))
                {
                    
                    Debug.Log("Found the two numbers: " + map[complement] + " and " + i);
                    return new int[] {map[complement], i};
                }

                map[nums[i]] = i;
            }
        
        return new int[0];
            
    }
}
